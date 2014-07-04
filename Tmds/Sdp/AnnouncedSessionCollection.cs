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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tmds.Sdp
{
    public class AnnouncedSessionCollection : IReadOnlyList<AnnouncedSession>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public AnnouncedSession this[int index]
        {
            get { return _sessions.Values[index]; }
        }

        public int Count
        {
            get { return _sessions.Count; }
        }

        public IEnumerator<AnnouncedSession> GetEnumerator()
        {
            return _sessions.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _sessions.Values.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        internal void Add(AnnouncedSession session)
        {
            _sessions.Add(new AnnouncedOrigin(session), session);
            int index = _sessions.Count - 1;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(CountString));
                PropertyChanged(this, new PropertyChangedEventArgs(IndexerName));
            }
            if (CollectionChanged != null)
            {
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, session, index);
                CollectionChanged(this, eventArgs);
            }
        }

        internal void Remove(AnnouncedSession session)
        {
            int index = _sessions.IndexOfKey(new AnnouncedOrigin(session));
            _sessions.RemoveAt(index);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(CountString));
                PropertyChanged(this, new PropertyChangedEventArgs(IndexerName));
            }
            if (CollectionChanged != null)
            {
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, session, index);
                CollectionChanged(this, eventArgs);
            }
        }

        internal void Replace(AnnouncedSession oldSession, AnnouncedSession newSession)
        {
            AnnouncedOrigin key = new AnnouncedOrigin(oldSession);
            int index = _sessions.IndexOfKey(key);
            _sessions[key] = newSession;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(IndexerName));
            }
            if (CollectionChanged != null)
            {
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldSession, newSession, index);
                CollectionChanged(this, eventArgs);
            }
        }

        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        private SortedList<AnnouncedOrigin, AnnouncedSession> _sessions = new SortedList<AnnouncedOrigin, AnnouncedSession>();
    }
}
