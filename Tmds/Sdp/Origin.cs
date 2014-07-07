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
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tmds.Sdp
{
    public class Origin
    {
        public Origin() :
            this(0, 0, IPAddress.Loopback)
        {}
        public Origin(ulong sessionID, ulong sessionVersion, IPAddress address)
        {
            if (address == null)
            {
                throw new ArgumentException("address");
            }
            UserName = "-";
            SessionID = sessionID;
            SessionVersion = sessionVersion;
            NetworkType = "IN";
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                AddressType = "IP4";
            }
            else
            {
                AddressType = "IP6";
            }
            UnicastAddress = address.ToString();
        }
        public Origin(string username, ulong sessionID, ulong sessionVersion, string networkType, string addressType, string unicastAddress)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username");
            }
            if (string.IsNullOrEmpty(networkType))
            {
                throw new ArgumentException("networkType");
            }
            if (string.IsNullOrEmpty(addressType))
            {
                throw new ArgumentException("addressType");
            }
            if (string.IsNullOrEmpty(unicastAddress))
            {
                throw new ArgumentException("unicastAddress");
            }
            UserName = username;
            SessionID = sessionID;
            SessionVersion = sessionVersion;
            NetworkType = networkType;
            AddressType = addressType;
            unicastAddress = UnicastAddress;
        }
        public SessionDescription SessionDescription { get; internal set; }
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
        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("value");
                }
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _userName = value;
            }
        }
        private ulong _sessionID;
        public ulong SessionID
        {
            get
            {
                return _sessionID;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _sessionID = value;
            }
        }
        private ulong _sessionVersion;
        public ulong SessionVersion
        {
            get
            {
                return _sessionVersion;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _sessionVersion = value;
            }
        }
        private string _networkType;
        public string NetworkType
        {
            get
            {
                return _networkType;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("value");
                }
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _networkType = value;
            }
        }
        private string _addressType;
        public string AddressType
        {
            get
            {
                return _networkType;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("value");
                }
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _addressType = value;
            }
        }
        public string _unicastAddress;
        public string UnicastAddress
        {
            get
            {
                return _networkType;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("value");
                }
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _unicastAddress = value;
            }
        }

        public bool IsSameSession(Origin o)
        {
            return ((SessionID.Equals(o.SessionID))
                && (UserName.Equals(o.UserName))
                && (NetworkType.Equals(o.NetworkType))
                && (AddressType.Equals(o.AddressType))
                && (UnicastAddress.Equals(o.UnicastAddress)));
        }
        public bool IsUpdateOf(Origin o)
        {
            if (IsSameSession(o))
            {
                return SessionVersion > o.SessionVersion;
            }
            return false;
        }
        public static bool operator ==(Origin lhs, Origin rhs)
        {
            if (Object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }
            if (Object.ReferenceEquals(lhs, null) || Object.ReferenceEquals(rhs, null))
            {
                return false;
            }
            return lhs.IsSameSession(rhs) && (lhs.SessionVersion == rhs.SessionVersion);
        }
        public static bool operator !=(Origin lhs, Origin rhs)
        {
            return !(lhs == rhs);
        }
        public override bool Equals(object obj)
        {
            Origin o = obj as Origin;
            if (o == null)
            {
                return false;
            }
            else
            {
                return this == o;
            }
        }
        public int GetSessionHashCode()
        {
            int h1 = UserName != null ? UserName.GetHashCode() : 0;
            int h2 = UnicastAddress != null ? UnicastAddress.GetHashCode() : 0;
            int h3 = SessionID.GetHashCode();
            int hash = 17;
            hash = hash * 31 + h1;
            hash = hash * 31 + h2;
            hash = hash * 31 + h3;
            return hash;
        }
        public override int GetHashCode()
        {
            int h4 = SessionVersion.GetHashCode();
            int hash = GetSessionHashCode();
            hash = hash * 31 + h4;
            return hash;
        }
    }
}
