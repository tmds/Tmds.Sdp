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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tmds.Sdp
{
    public class SessionDescription
    {
        public SessionDescription(string name, Origin origin)
        {
            if (string.IsNullOrEmpty("name"))
            {
                throw new ArgumentException("name");
            }
            if (!origin.IsValid)
            {
                throw new ArgumentException("origin");
            }
            Version = 0;
            Origin = origin;
            Name = name;
        }

        public bool IsReadOnly { get; private set; }
        public void SetReadOnly()
        {
            IsReadOnly = true;
        }

        private int _version;
        public int Version
        {
            get
            {
                return _version;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is Read-only");
                }
                _version = value;
            }
        }
        internal bool HasOrigin
        {
            get
            {
                return _origin.HasValue;
            }
        }
        private Origin? _origin;
        public Origin Origin
        {
            get
            {
                return _origin.Value;
            }
            set
            {
                if (!value.IsValid)
                {
                    throw new ArgumentException("origin");
                }
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is Read-only");
                }
                _origin = value;
            }
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
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
                _name = value;
            }
        }
        public string _information;
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
        private string _uri;
        public string Uri
        {
            get
            {
                return _uri;
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
                _uri = value;
            }
        }
        public bool HasEMails
        {
            get
            {
                return ((_emails != null) && (_emails.Count != 0));
            }
        }
        private StringCollection _emails;
        public IList<string> EMails
        {
            get
            {
                if (_emails == null)
                {
                    _emails = new StringCollection(StringCollection.Type.EMail, this);
                }
                return _emails;
            }
        }
        public bool HasPhoneNumbers
        {
            get
            {
                return ((_phoneNumbers != null) && (_phoneNumbers.Count != 0));
            }
        }
        private StringCollection _phoneNumbers;
        public IList<string> PhoneNumbers
        {
            get
            {
                if (_phoneNumbers == null)
                {
                    _phoneNumbers = new StringCollection(StringCollection.Type.Phone, this);
                }
                return _phoneNumbers;
            }
        }
        public bool HasConnection
        {
            get
            {
                return _connection.HasValue;
            }
        }
        private Connection? _connection;
        public Connection Connection
        {
            get
            {
                return _connection.Value;
            }
            set
            {
                if (!value.IsValid)
                {
                    throw new ArgumentException("value");
                }
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is Read-only");
                }
                _connection = value;
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
        public bool HasTimes
        {
            get
            {
                return ((_times != null) && (_attributes.Count != 0));
            }
        }
        private TimeCollection _times;
        public IList<Time> Times
        {
            get
            {
                if (_times == null)
                {
                    _times = new TimeCollection(this);
                }
                return _times;
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
        public bool HasMedias
        {
            get
            {
                return ((_medias != null) && (_medias.Count != 0));
            }
        }
        private MediaCollection _medias;
        public IList<Media> Medias
        {
            get
            {
                if (_medias == null)
                {
                    _medias = new MediaCollection(this);
                }
                return _medias;
            }
        }
        public bool IsSameSession(SessionDescription sd)
        {
            if (sd == null)
            {
                return false;
            }
            return ((Origin.SessionID.Equals(sd.Origin.SessionID))
                && (Origin.UserName.Equals(sd.Origin.UserName))
                && (Origin.NetworkType.Equals(sd.Origin.NetworkType))
                && (Origin.AddressType.Equals(sd.Origin.AddressType))
                && (Origin.UnicastAddress.Equals(sd.Origin.UnicastAddress)));
        }
        public bool IsUpdateOf(SessionDescription sd)
        {
            if (sd == null)
            {
                return false;
            }
            return Origin.IsUpdateOf(sd.Origin);
        }
        public static bool operator ==(SessionDescription lhs, SessionDescription rhs)
        {
            if (Object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }
            if (Object.ReferenceEquals(lhs, null) || Object.ReferenceEquals(rhs, null))
            {
                return false;
            }
            if (!lhs.HasOrigin || !rhs.HasOrigin)
            {
                return false;
            }
            return lhs.Origin == rhs.Origin;
        }
        public static bool operator !=(SessionDescription lhs, SessionDescription rhs)
        {
            return !(lhs == rhs);
        }
        public override bool Equals(object obj)
        {
            SessionDescription sd = obj as SessionDescription;
            if (sd == null)
            {
                return false;
            }
            else
            {
                return this == sd;
            }
        }
        public override int GetHashCode()
        {
            return Origin.GetHashCode();
        }
        public void Save(TextWriter textWriter)
        {
            textWriter.Write(ToString());
        }
        public void Save(Stream stream)
        {
            Save(new StreamWriter(stream));
        }
        public static SessionDescription Parse(string text)
        {
            return Parse(text, LoadOptions.Default);
        }
        public static SessionDescription Parse(string text, LoadOptions options)
        {
            return Load(new StringReader(text), options);
        }
        public static SessionDescription Load(Stream stream)
        {
            return Load(stream, LoadOptions.Default);
        }
        public static SessionDescription Load(Stream stream, LoadOptions loadOptions)
        {
            return Load(new StreamReader(stream), loadOptions);
        }
        public static SessionDescription Load(TextReader reader)
        {
            return Load(reader, LoadOptions.Default);
        }
        public static SessionDescription Load(TextReader reader, LoadOptions loadOptions)
        {
            SessionDescription sd = new SessionDescription();

            Media media = null;
            string line = reader.ReadLine();
            while (line != null)
            {
                if ((line.Length == 0) && ((loadOptions & LoadOptions.IgnoreEmptyLines) != 0))
                {
                    goto nextline;
                }
                if (line.Length < 3)
                {
                    goto invalidline;
                }
                if (line[1] != '=')
                {
                    goto invalidline;
                }
                string value = line.Substring(2);
                string[] parts = null;
                int sep = -1;
                try
                {
                    switch (line[0])
                    {
                        case 'v':
                            sd.Version = -1;
                            if (media != null)
                            {
                                goto invalidline;
                            }
                            int version = -1;
                            if (!int.TryParse(value, out version))
                            {
                                goto invalidline;
                            }
                            if ((loadOptions & LoadOptions.IgnoreUnsupportedVersion) == 0)
                            {
                                if (version != 0)
                                {
                                    goto notsupported;
                                }
                            }
                            sd.Version = version;
                            break;
                        case 'o':
                            Origin origin = new Origin();
                            if (media != null)
                            {
                                goto invalidline;
                            }
                            parts = value.Split(' ');
                            if (parts.Length != 6)
                            {
                                goto invalidline;
                            }
                            origin.UserName = parts[0];
                            ulong sessionID = 0;
                            if (!ulong.TryParse(parts[1], out sessionID))
                            {
                                goto invalidline;
                            }
                            origin.SessionID = sessionID;
                            ulong sessionVersion = 0;
                            if (!ulong.TryParse(parts[2], out sessionVersion))
                            {
                                goto invalidline;
                            }
                            origin.SessionVersion = sessionVersion;
                            origin.NetworkType = parts[3];
                            origin.AddressType = parts[4];
                            origin.UnicastAddress = parts[5];
                            sd.Origin = origin;
                            break;
                        case 's':
                            if (media != null)
                            {
                                goto invalidline;
                            }
                            sd.Name = value;
                            break;
                        case 'i':
                            if (media == null)
                            {
                                sd.Information = value;
                            }
                            else
                            {
                                media.Information = value;
                            }
                            break;
                        case 'u':
                            if (media != null)
                            {
                                goto invalidline;
                            }
                            sd.Uri = value;
                            break;
                        case 'e':
                            if (media != null)
                            {
                                goto invalidline;
                            }
                            sd.EMails.Add(value);
                            break;
                        case 'p':
                            if (media != null)
                            {
                                goto invalidline;
                            }
                            sd.PhoneNumbers.Add(value);
                            break;
                        case 'c':
                            if (media == null)
                            {
                                if (sd.HasConnection)
                                {
                                    goto invalidline;
                                }
                            }
                            Connection conn = new Connection();
                            parts = value.Split(' ');
                            if (parts.Length != 3)
                            {
                                goto invalidline;
                            }
                            conn.NetworkType = parts[0];
                            conn.AddressType = parts[1];
                            parts = parts[2].Split('/');
                            if (parts.Length > 3)
                            {
                                goto invalidline;
                            }
                            conn.Address = parts[0];
                            conn.AddressCount = 1;
                            if (parts.Length >= 2)
                            {
                                uint ttl = 0;
                                if (!uint.TryParse(parts[1], out ttl))
                                {
                                    goto invalidline;
                                }
                                conn.Ttl = ttl;
                                if (parts.Length == 3)
                                {
                                    uint addressCount = 0;
                                    if (!uint.TryParse(parts[2], out addressCount))
                                    {
                                        goto invalidline;
                                    }
                                    conn.AddressCount = addressCount;
                                }
                                if (conn.AddressType == "IP6")
                                {
                                    if (parts.Length == 3)
                                    {
                                        goto invalidline;
                                    }
                                    conn.AddressCount = conn.Ttl;
                                    conn.Ttl = 0;
                                }
                            }

                            if (media != null)
                            {
                                media.Connections.Add(conn);
                            }
                            else
                            {
                                sd.Connection = conn;
                            }
                            break;
                        case 'b':
                            sep = value.IndexOf(':');
                            if (sep == -1)
                            {
                                goto invalidline;
                            }
                            string type = value.Substring(0, sep);
                            uint bwValue = 0;
                            if (!uint.TryParse(value.Substring(sep + 1), out bwValue))
                            {
                                goto invalidline;
                            }
                            if (media != null)
                            {
                                media.Bandwidths.Add(new Bandwidth(type, bwValue));
                            }
                            else
                            {
                                sd.Bandwidths.Add(new Bandwidth(type, bwValue));
                            }
                            break;
                        case 'a':
                            sep = value.IndexOf(':');
                            string attrName;
                            string attrValue;
                            if (sep != -1)
                            {
                                attrName = value.Substring(0, sep);
                                attrValue = value.Substring(sep + 1);
                            }
                            else
                            {
                                attrName = value;
                                attrValue = null;
                            }
                            if (media != null)
                            {
                                media.Attributes.Add(attrName, attrValue);
                            }
                            else
                            {
                                sd.Attributes.Add(attrName, attrValue);
                            }
                            break;
                        case 't':
                            if (media != null)
                            {
                                goto invalidline;
                            }
                            parts = value.Split(' ');
                            if (parts.Length != 2)
                            {
                                goto invalidline;
                            }
                            ulong startTime = 0;
                            if (!ulong.TryParse(parts[0], out startTime))
                            {
                                goto invalidline;
                            }
                            ulong stopTime = 0;
                            if (!ulong.TryParse(parts[1], out stopTime))
                            {
                                goto invalidline;
                            }
                            sd.Times.Add(new Time(startTime, stopTime));
                            break;
                        case 'm':
                            media = new Media();

                            parts = value.Split(' ');
                            if (parts.Length != 4)
                            {
                                goto invalidline;
                            }
                            media.Type = parts[0];
                            media.Protocol = parts[2];
                            media.Format = parts[3];

                            parts = parts[1].Split('/');
                            if (parts.Length > 2)
                            {
                                goto invalidline;
                            }
                            uint port = 0;
                            media.PortCount = 1;
                            if (!uint.TryParse(parts[0], out port))
                            {
                                goto invalidline;
                            }
                            media.Port = port;
                            if (parts.Length == 2)
                            {
                                uint portCount = 0;
                                if (!uint.TryParse(parts[1], out portCount))
                                {
                                    goto invalidline;
                                }
                                media.PortCount = portCount;
                            }
                            sd.Medias.Add(media);
                            break;
                        case 'z':
                        case 'k':
                        case 'r':
                            if ((loadOptions & LoadOptions.IgnoreUnsupportedLines) == 0)
                            {
                                goto notsupported;
                            }
                            break;
                        default:
                            if ((loadOptions & LoadOptions.IgnoreUnknownLines) == 0)
                            {
                                goto invalidline;
                            }
                            break;
                    }
                }
                catch
                {
                    goto invalidline;
                }
            nextline:
                line = reader.ReadLine();
                continue;
            invalidline:
                throw new SdpException(string.Format("Invalid Line {0}", line));
            notsupported:
                throw new SdpException(string.Format("Unsupported line {0}", line));
            }

            if (sd.Version == -1)
            {
                throw new SdpException("Version ('v=') is required");
            }
            if (!sd.HasOrigin)
            {
                throw new SdpException("Origin ('o=') is required");
            }
            if (sd.Name == null)
            {
                throw new SdpException("Name ('s=') is required");
            }
            return sd;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("v=");
            sb.Append(Version);
            sb.Append("\r\n");

            sb.Append("o=");
            sb.Append(Origin.UserName);
            sb.Append(' ');
            sb.Append(Origin.SessionID);
            sb.Append(' ');
            sb.Append(Origin.SessionVersion);
            sb.Append(' ');
            sb.Append(Origin.NetworkType);
            sb.Append(' ');
            sb.Append(Origin.AddressType);
            sb.Append(' ');
            sb.Append(Origin.UnicastAddress);
            sb.Append("\r\n");

            sb.Append("s=");
            sb.Append(Name);
            sb.Append("\r\n");

            if (Information != null)
            {
                sb.Append("i=");
                sb.Append(Information);
                sb.Append("\r\n");
            }

            if (Uri != null)
            {
                sb.Append("u=");
                sb.Append(Uri);
                sb.Append("\r\n");
            }

            foreach (string email in EMails)
            {
                sb.Append("e=");
                sb.Append(email);
                sb.Append("\r\n");
            }
            foreach (string phone in PhoneNumbers)
            {
                sb.Append("p=");
                sb.Append(phone);
                sb.Append("\r\n");
            }
            if (HasConnection)
            {
                sb.Append("c=");
                sb.Append(Connection.NetworkType);
                sb.Append(' ');
                sb.Append(Connection.AddressType);
                sb.Append(' ');
                sb.Append(Connection.Address);
                if (Connection.Ttl != 0)
                {
                    sb.Append('/');
                    sb.Append(Connection.Ttl);
                }
                if (Connection.AddressCount != 0)
                {
                    sb.Append('/');
                    sb.Append(Connection.AddressCount);
                }
                sb.Append("\r\n");
            }
            foreach (Bandwidth bw in Bandwidths)
            {
                sb.Append("b=");
                sb.Append(bw.Type);
                sb.Append(' ');
                sb.Append(bw.Value);
                sb.Append("\r\n");
            }
            foreach (Time time in Times)
            {
                sb.Append("t=");
                sb.Append(time.StartTime);
                sb.Append(' ');
                sb.Append(time.StopTime);
                sb.Append("\r\n");
            }
            foreach (var attr in Attributes)
            {
                sb.Append("a=");
                sb.Append(attr.Key);
                if (attr.Value != null)
                {
                    sb.Append(':');
                    sb.Append(attr.Value);
                }
                sb.Append("\r\n");
            }
            foreach (Media media in Medias)
            {
                sb.Append("m=");
                sb.Append(media.Type);
                sb.Append(' ');
                sb.Append(media.Port);
                if (media.PortCount != 1)
                {
                    sb.Append('/');
                    sb.Append(media.PortCount);
                }
                sb.Append(' ');
                sb.Append(media.Protocol);
                sb.Append(' ');
                sb.Append(media.Format);
                sb.Append("\r\n");

                if (media.Information != null)
                {
                    sb.Append("i=");
                    sb.Append(media.Information);
                    sb.Append("\r\n");
                }

                foreach (Connection conn in media.Connections)
                {
                    sb.Append("c=");
                    sb.Append(conn.NetworkType);
                    sb.Append(' ');
                    sb.Append(conn.AddressType);
                    sb.Append(' ');
                    sb.Append(conn.Address);
                    if (conn.Ttl != 0)
                    {
                        sb.Append('/');
                        sb.Append(conn.Ttl);
                    }
                    if (conn.AddressCount != 1)
                    {
                        sb.Append('/');
                        sb.Append(conn.AddressCount);
                    }
                    sb.Append("\r\n");
                }
                foreach (Bandwidth bw in media.Bandwidths)
                {
                    sb.Append("b=");
                    sb.Append(bw.Type);
                    sb.Append(' ');
                    sb.Append(bw.Value);
                    sb.Append("\r\n");
                }
                foreach (var attr in media.Attributes)
                {
                    sb.Append("a=");
                    sb.Append(attr.Key);
                    if (attr.Value != null)
                    {
                        sb.Append(':');
                        sb.Append(attr.Value);
                    }
                    sb.Append("\r\n");
                }
            }
            return sb.ToString();
        }

        internal SessionDescription()
        { }
    }
}
