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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tmds.Sdp
{
    public class Media
    {
        public static string TypeAudio = "audio";
        public static string TypeVideo = "video";
        public static string TypeText = "text";
        public static string TypeApplication = "application";
        public static string TypeMessage = "message";

        public static string ProtocolUdp = "udp";
        public static string ProtocolRtpAvp = "RTP/AVP";
        public static string ProtocolRtpSavp = "RTP/SAVP";

        public Media(string type, uint port, uint portCount, string protocol, string format)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("type");
            }
            if (string.IsNullOrEmpty(protocol))
            {
                throw new ArgumentException("protocol");
            }
            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentException("format");
            }

            Type = type;
            Port = port;
            PortCount = portCount;
            Protocol = protocol;
            Format = format;
        }

        public bool IsReadOnly
        {
            get
            {
                if (SessionDescription != null)
                {
                    return SessionDescription.IsReadOnly;
                }
                else
                {
                    return false;
                }
            }
        }

        private string _type;
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("value");
                }
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is Read-only");
                }
                _type = value;
            }
        }
        private uint _port;
        public uint Port
        {
            get
            {
                return _port;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is Read-only");
                }
                _port = value;
            }
        }
        private uint _portCount;
        public uint PortCount
        {
            get
            {
                return _portCount;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is Read-only");
                }
                _portCount = value;
            }
        }
        private string _protocol;
        public string Protocol
        {
            get
            {
                return _protocol;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("value");
                }
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is Read-only");
                }
                _protocol = value;
            }
        }
        private string _format;
        public string Format
        {
            get
            {
                return _format;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("value");
                }
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is Read-only");
                }
                _format = value;
            }
        }
        private string _information;
        public string Information
        {
            get
            {
                return _information;
            }
            set
            {
                if (value == string.Empty)
                {
                    throw new ArgumentException("value");
                }
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is Read-only");
                }
                _information = value;
            }
        }
        public SessionDescription SessionDescription { get; internal set; }
        public bool HasConnections
        {
            get
            {
                return ((_connections != null) && (_connections.Count != 0));
            }
        }
        private ConnectionCollection _connections;
        public IList<Connection> Connections
        {
            get
            {
                if (_connections == null)
                {
                    _connections = new ConnectionCollection(this);
                }
                return _connections;
            }
        }
        public bool HasBandwidths
        {
            get
            {
                return ((_bandwidths != null) && (_bandwidths.Count != 0));
            }
        }
        private BandwidthCollection _bandwidths;
        public IList<Bandwidth> Bandwidths
        {
            get
            {
                if (_bandwidths == null)
                {
                    _bandwidths = new BandwidthCollection(this);
                }
                return _bandwidths;
            }
        }
        public bool HasAttributes
        {
            get
            {
                return ((_attributes != null) && (_attributes.Count != 0));
            }
        }
        private AttributeDictionary _attributes;
        public AttributeDictionary Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new AttributeDictionary(this);
                }
                return _attributes;
            }
        }

        internal Media()
        { }
    }
}
