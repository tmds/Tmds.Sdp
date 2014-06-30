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
            _sessionData = new Dictionary<AnnouncedOrigin, SessionData>();
            Sessions = new AnnouncedSessionCollection();
        }

        public bool IsEnabled { get; private set; }
        public SynchronizationContext SynchronizationContext { get; private set; }
        public int DefaultTimeOut { get; set; }
        public AnnouncedSessionCollection Sessions { get; private set; }

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
                var networkInterface = new NetworkInterface(interfaceInfo);
                int index = interfaceInfo.GetIPProperties().GetIPv4Properties().Index;
                _interfaceHandlers.Add(index, new NetworkInterfaceHandler(this, networkInterface));
            }
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
                AnnouncedOrigin announcedOrigin = new AnnouncedOrigin(sd.Origin, interfaceHandler.NetworkInterface);
                SessionData sessionData = null;
                AnnouncedSession announcedSession = null;
                if (_sessionData.TryGetValue(announcedOrigin, out sessionData))
                {
                    announcedSession = sessionData.Session;
                }

                if (sessionData != null)
                {
                    if (sd.IsUpdateOf(announcedSession.SessionDescription))
                    {
                        announcedSession = new AnnouncedSession(sd, interfaceHandler.NetworkInterface);
                        AnnouncedSession oldSession = sessionData.Session;
                        sessionData.Session = announcedSession;
                        SynchronizationContextPost(o =>
                        {
                            lock (Sessions)
                            {
                                Sessions.Replace(oldSession, announcedSession);
                            }
                        });
                    }

                }
                else
                {
                    announcedSession = new AnnouncedSession(sd, interfaceHandler.NetworkInterface);
                    sessionData = new SessionData()
                    {
                        Session = announcedSession,
                        Timer = new Timer(OnTimeOut, announcedOrigin, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite)
                    };
                    _sessionData.Add(announcedOrigin, sessionData);
                    SynchronizationContextPost(o =>
                    {
                        lock (Sessions)
                        {
                            Sessions.Add(announcedSession);
                        }
                    });
                }
                sessionData.TimeOutTime = DateTime.Now + TimeSpan.FromMilliseconds(DefaultTimeOut);
                sessionData.Timer.Change(DefaultTimeOut, System.Threading.Timeout.Infinite);
            }
        }

        internal void OnSessionDelete(NetworkInterfaceHandler interfaceHandler, SessionDescription sd)
        {
            lock (_sessionData)
            {
                AnnouncedOrigin announcedOrigin = new AnnouncedOrigin(sd.Origin, interfaceHandler.NetworkInterface);
                SessionData sessionData = null;
                AnnouncedSession announcedSession = null;
                if (_sessionData.TryGetValue(announcedOrigin, out sessionData))
                {
                    announcedSession = sessionData.Session;
                }

                if (sessionData != null)
                {
                    if (sd.IsUpdateOf(announcedSession.SessionDescription))
                    {
                        sessionData.Timer.Dispose();
                        _sessionData.Remove(announcedOrigin);
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
                var removedSessions = _sessionData.Where(s => s.Key.Interface == networkInterfaceHandler.NetworkInterface).ToList();
                foreach (var pair in removedSessions)
                {
                    AnnouncedOrigin announcedOrigin = pair.Key;
                    SessionData sessionData = pair.Value;
                    sessionData.Timer.Dispose();
                    _sessionData.Remove(announcedOrigin);
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

        private class SessionData
        {
            public DateTime TimeOutTime;
            public Timer Timer;
            public AnnouncedSession Session;
        }
        private class AnnouncedOrigin
        {
            public NetworkInterface Interface { get; private set; }
            public Origin Origin { get; private set; }
            public AnnouncedOrigin(Origin origin, NetworkInterface inter)
            {
                Origin = origin;
                Interface = inter;
            }
            public override int GetHashCode()
            {
                int h1 = Interface.GetHashCode();
                int h2 = Origin.GetSessionHashCode();
                int hash = 17;
                hash = hash * 31 + h1;
                hash = hash * 31 + h2;
                return hash;
            }
            public override bool Equals(object obj)
            {
                AnnouncedOrigin ao = obj as AnnouncedOrigin;
                if (ao == null)
                {
                    return false;
                }
                return (Interface.Equals(ao.Interface) && Origin.IsSameSession(ao.Origin));
            }
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
                AnnouncedOrigin announcedOrigin = state as AnnouncedOrigin;
                SessionData sessionData = null;
                _sessionData.TryGetValue(announcedOrigin, out sessionData);
                if (sessionData != null)
                {
                    if (sessionData.TimeOutTime > DateTime.Now)
                    {
                        return;
                    }
                    sessionData.Timer.Dispose();
                    _sessionData.Remove(announcedOrigin);
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
                    if (interfaceHandler != null)
                    {
                        if (interfaceInfo.OperationalStatus == OperationalStatus.Up)
                        {
                            interfaceHandler.Enable();
                        }
                        else
                        {
                            interfaceHandler.Disable();
                        }
                    }
                }
            }
        }

        private Dictionary<int, NetworkInterfaceHandler> _interfaceHandlers;
        private Dictionary<AnnouncedOrigin, SessionData> _sessionData;
    }
}
