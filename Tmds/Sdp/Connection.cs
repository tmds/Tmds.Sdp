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
    public struct Connection
    {
        public Connection(IPAddress address, uint addressCount, uint ttl) :
            this()
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
        public Connection(string networkType, string addressType, string address, uint addressCount, uint ttl) :
            this()
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
                _addressType = value;
            }
        }
        private string _address;
        public string Address
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
                _address = value;
            }
        }
        public uint AddressCount { get; set; }
        public uint Ttl { get; set; }

        internal bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(NetworkType))
                {
                    return false;
                }
                if (string.IsNullOrEmpty(AddressType))
                {
                    return false;
                }
                if (string.IsNullOrEmpty(Address))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
