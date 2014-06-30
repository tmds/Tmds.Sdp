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
    class StringCollection : Collection<string>
    {
        public enum Type
        {
            Phone,
            EMail
        }
        public StringCollection(Type type, SessionDescription sessionDescription)
        {
            SessionDescription = sessionDescription;
        }
        public SessionDescription SessionDescription { get; private set; }
        public bool IsReadOnly
        {
            get
            {
                return SessionDescription.IsReadOnly;
            }
        }
        protected override void InsertItem(int index, string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                throw new ArgumentNullException("item");
            }
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is Read-only");
            }
            base.InsertItem(index, item);
        }
        protected override void SetItem(int index, string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                throw new ArgumentNullException("item");
            }
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is Read-only");
            }
            base.SetItem(index, item);
        }
        protected override void ClearItems()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is Read-only");
            }
            base.ClearItems();
        }
        protected override void RemoveItem(int index)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is Read-only");
            }
            base.RemoveItem(index);
        }
    }
}
