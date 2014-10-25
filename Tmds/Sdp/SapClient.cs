//Copyright (C) 2014  Tom Deseyn

//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.

//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkInterfaceInformation = System.Net.NetworkInformation.NetworkInterface;

namespace Tmds.Sdp
{
    public class SapClient
    {
        private static SapClient _sapClient;
        public static SapClient Default
        {
            get { return _sapClient; }
        }
        static SapClient()
        {
            _sapClient = new SapClient();
        }

        public SapClient()
        {
            DefaultTimeOut = TimeSpan.FromHours(1);
            _sessionData = new Dictionary<SdpSession, SessionData>();
            Sessions = new AnnouncedSessionCollection();
        }

        public bool IsEnabled { get; private set; }
        public SynchronizationContext SynchronizationContext { get; private set; }
        public TimeSpan DefaultTimeOut { get; set; }
        public AnnouncedSessionCollection Sessions { get; private set; }

        public delegate void ExceptionEventHandler(Object sender, ExceptionEventArgs e);

        public event ExceptionEventHandler Exception;

        public void Enable(SynchronizationContext synchronizationContext)
        {
            lock (_sessionData)
            {
                if (IsEnabled)
                {
                    if ((synchronizationContext != null) && (SynchronizationContext != synchronizationContext))
                    {
                        throw new Exception("Cannot use two different SynchronizationContexts");
                    }
                    return;
                }
                IsEnabled = true;
            }

            SynchronizationContext = synchronizationContext;

            _interfaceHandlers = new Dictionary<int, NetworkInterfaceHandler>();
            NetworkChange.NetworkAddressChanged += CheckNetworkInterfaceStatuses;
            CheckNetworkInterfaceStatuses(null, null);
        }

        public void Enable(bool useSynchronizationContext = true)
        {
            if (useSynchronizationContext)
            {
                Enable(SynchronizationContext.Current);
            }
            else
            {
                Enable(null);
            }
        }

        internal void OnSessionAnnounce(NetworkInterfaceHandler interfaceHandler, SessionDescription sd)
        {
            lock (_sessionData)
            {
                SdpSession session = new SdpSession(sd, interfaceHandler.NetworkInterface);
                SessionData sessionData = null;
                SessionAnnouncement sessionAnnouncement = null;
                if (_sessionData.TryGetValue(session, out sessionData))
                {
                    sessionAnnouncement = sessionData.Session;
                }

                if (sessionData != null)
                {
                    if (sd.IsUpdateOf(sessionAnnouncement.SessionDescription))
                    {
                        sessionAnnouncement = new SessionAnnouncement(sd, interfaceHandler.NetworkInterface);
                        SessionAnnouncement oldAnnouncement = sessionData.Session;
                        sessionData.Session = sessionAnnouncement;
                        SynchronizationContextPost(o =>
                        {
                            lock (Sessions)
                            {
                                Sessions.Replace(oldAnnouncement, sessionAnnouncement);
                            }
                        });
                    }

                }
                else
                {
                    sessionAnnouncement = new SessionAnnouncement(sd, interfaceHandler.NetworkInterface);
                    sessionData = new SessionData()
                    {
                        Session = sessionAnnouncement,
                        Timer = new Timer(OnTimeOut, session, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite)
                    };
                    _sessionData.Add(session, sessionData);
                    SynchronizationContextPost(o =>
                    {
                        lock (Sessions)
                        {
                            Sessions.Add(sessionAnnouncement);
                        }
                    });
                }
                sessionData.TimeOutTime = DateTime.Now + DefaultTimeOut;
                sessionData.Timer.Change(DefaultTimeOut, TimeSpan.FromMilliseconds(-1));
            }
        }

        internal void OnSessionDelete(NetworkInterfaceHandler interfaceHandler, Origin origin)
        {
            lock (_sessionData)
            {
                SdpSession session = new SdpSession(origin, interfaceHandler.NetworkInterface);
                SessionData sessionData = null;
                SessionAnnouncement sessionAnnouncement = null;
                if (_sessionData.TryGetValue(session, out sessionData))
                {
                    sessionAnnouncement = sessionData.Session;
                }

                if (sessionData != null)
                {
                    if (origin.IsUpdateOrEqual(sessionAnnouncement.SessionDescription.Origin))
                    {
                        sessionData.Timer.Dispose();
                        _sessionData.Remove(session);
                        SynchronizationContextPost(o =>
                        {
                            lock (Sessions)
                            {
                                Sessions.Remove(sessionData.Session);
                            }
                        });
                    }
                }
            }
        }

        internal void OnInterfaceDisable(NetworkInterfaceHandler networkInterfaceHandler)
        {
            lock (_sessionData)
            {
                var removedSessions = _sessionData.Where(s => s.Key.NetworkInterface == networkInterfaceHandler.NetworkInterface).ToList();
                foreach (var pair in removedSessions)
                {
                    SdpSession session = pair.Key;
                    SessionData sessionData = pair.Value;
                    sessionData.Timer.Dispose();
                    _sessionData.Remove(session);
                    SynchronizationContextPost(o =>
                    {
                        lock (Sessions)
                        {
                            Sessions.Remove(sessionData.Session);
                        }
                    });
                }
            }
        }

        internal void OnException(Exception e)
        {
            SynchronizationContextPost(o =>
            {
                if (Exception != null)
                {
                    Exception(this, new ExceptionEventArgs(e));
                }
            });
        }

        private class SessionData
        {
            public DateTime TimeOutTime;
            public Timer Timer;
            public SessionAnnouncement Session;
        }

        private void SynchronizationContextPost(SendOrPostCallback cb)
        {
            if (SynchronizationContext != null)
            {
                SynchronizationContext.Post(cb, null);
            }
            else
            {
                cb(null);
            }
        }

        private void OnTimeOut(object state)
        {
            lock (_sessionData)
            {
                SdpSession session = state as SdpSession;
                SessionData sessionData = null;
                _sessionData.TryGetValue(session, out sessionData);
                if (sessionData != null)
                {
                    if (sessionData.TimeOutTime > DateTime.Now)
                    {
                        return;
                    }
                    sessionData.Timer.Dispose();
                    _sessionData.Remove(session);
                    SynchronizationContextPost(o =>
                    {
                        lock (Sessions)
                        {
                            Sessions.Remove(sessionData.Session);
                        }
                    });
                }
            }
        }

        private void CheckNetworkInterfaceStatuses(object sender, EventArgs e)
        {
            lock (_interfaceHandlers)
            {
                HashSet<NetworkInterfaceHandler> handlers = new HashSet<NetworkInterfaceHandler>(_interfaceHandlers.Values);
                NetworkInterfaceInformation[] interfaceInfos = NetworkInterfaceInformation.GetAllNetworkInterfaces();
                foreach (NetworkInterfaceInformation interfaceInfo in interfaceInfos)
                {
                    if (interfaceInfo.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    {
                        continue;
                    }
                    if (interfaceInfo.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                    {
                        continue;
                    }
                    int index = interfaceInfo.GetIPProperties().GetIPv4Properties().Index;
                    NetworkInterfaceHandler interfaceHandler;
                    _interfaceHandlers.TryGetValue(index, out interfaceHandler);
                    if (interfaceHandler == null)
                    {
                        var networkInterface = new NetworkInterface(interfaceInfo);
                        index = interfaceInfo.GetIPProperties().GetIPv4Properties().Index;
                        interfaceHandler = new NetworkInterfaceHandler(this, networkInterface);
                        _interfaceHandlers.Add(index, interfaceHandler);
                    }
                    if (interfaceInfo.OperationalStatus == OperationalStatus.Up)
                    {
                        interfaceHandler.Enable();
                    }
                    else
                    {
                        interfaceHandler.Disable();
                    }
                    handlers.Remove(interfaceHandler);
                }
                foreach (NetworkInterfaceHandler handler in handlers)
                {
                    handler.Disable();
                }
            }
        }

        private Dictionary<int, NetworkInterfaceHandler> _interfaceHandlers;
        private Dictionary<SdpSession, SessionData> _sessionData;
    }
}
