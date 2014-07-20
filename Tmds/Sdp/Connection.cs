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
    public class Connection
    {
        public Connection() :
            this(IPAddress.Any, 1, 0)
        {}

        public Connection(IPAddress address, uint addressCount, uint ttl)
        {
            if (address == null)
            {
                throw new ArgumentException("address");
            }
            AddressCount = addressCount;
            NetworkType = "IN";
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                AddressType = "IP4";
                Ttl = ttl;
            }
            else
            {
                AddressType = "IP6";
                Ttl = 0;
            }
            Address = address.ToString();
        }
        public Connection(string networkType, string addressType, string address, uint addressCount, uint ttl)
        {
            if (string.IsNullOrEmpty(networkType))
            {
                throw new ArgumentException("networkType");
            }
            if (string.IsNullOrEmpty(addressType))
            {
                throw new ArgumentException("addressType");
            }
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException("address");
            }
            NetworkType = networkType;
            AddressType = addressType;
            Address = address;
            AddressCount = addressCount;
            Ttl = ttl;
        }

        public SessionDescription SessionDescription { get; internal set; }
        public Media Media { get; internal set; }
        public bool IsReadOnly
        {
            get
            {
                if (Media != null && Media.IsReadOnly)
                {
                    return true;
                }
                if (SessionDescription != null && SessionDescription.IsReadOnly)
                {
                    return true;
                }
                return false;
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
                return _addressType;
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
        private string _address;
        public string Address
        {
            get
            {
                return _address;
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
                _address = value;
            }
        }
        private uint _addressCount;
        public uint AddressCount
        {
            get
            {
                return _addressCount;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _addressCount = value;
            }
        }
        private uint _ttl;
        public uint Ttl
        {
            get
            {
                return _ttl;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("SessionDescription is read-only");
                }
                _ttl = value;
            }
        }
    }
}
