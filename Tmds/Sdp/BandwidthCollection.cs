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
using System.Text;
using System.Threading.Tasks;

namespace Tmds.Sdp
{
    class BandwidthCollection : Collection<Bandwidth>
    {
        public BandwidthCollection(SessionDescription sessionDescription)
        {
            SessionDescription = sessionDescription;
        }
        public BandwidthCollection(Media media)
        {
            Media = media;
        }

        public SessionDescription SessionDescription { get; private set; }
        public Media Media { get; private set; }

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

        protected override void InsertItem(int index, Bandwidth item)
        {
            if (!item.IsValid)
            {
                throw new ArgumentException("item");
            }
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.InsertItem(index, item);
        }
        protected override void SetItem(int index, Bandwidth item)
        {
            if (!item.IsValid)
            {
                throw new ArgumentException("item");
            }
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.SetItem(index, item);
        }
        protected override void ClearItems()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.ClearItems();
        }
        protected override void RemoveItem(int index)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            base.RemoveItem(index);
        }
    }
}
