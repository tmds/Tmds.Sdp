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
    public class AttributeDictionary
    {
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

        public void Add(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            _values.Add(new KeyValuePair<string, string>(name, value));
        }

        public void Add(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            _values.Add(new KeyValuePair<string, string>(name, null));
        }

        public void Set(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            if (ContainsKey(name))
            {
                throw new ArgumentException("An element with the same name already exists");
            }
            Add(name, value);
        }

        public void Set(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            Set(name, (string)null);
        }

        public void Set(string name, IEnumerable<string> values)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            if (ContainsKey(name))
            {
                throw new ArgumentException("An element with the same name already exists");
            }
            foreach (var value in values)
            {
                Add(name, value);
            }
        }

        public bool ContainsKey(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            foreach (var pair in _values)
            {
                if (pair.Key.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Remove(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            int removed = 0;
            while (_values.Remove(new KeyValuePair<string, string>(name, value)))
            {
                removed++;
            }
            return removed != 0;
        }

        public bool Remove(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            var attributes = _values.Where(pair => pair.Key.Equals(name)).ToArray();

            foreach (var pair in attributes)
            {
                _values.Remove(pair);
            }

            return attributes.Length != 0;
        }

        public bool TryGetValue(string name, out string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            var attributes = _values.Where(pair => pair.Key.Equals(name)).ToList();
            if (attributes.Count == 1)
            {
                value = attributes[0].Value;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                HashSet<string> names = new HashSet<string>();
                foreach (var attribute in _values)
                {
                    names.Add(attribute.Key);
                }
                return names;
            }
        }

        public string GetValue(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            string value;
            if (!TryGetValue(name, out value))
            {
                throw new InvalidOperationException("No or multiple attributes with that name");
            }
            return value;
        }

        public IList<string> GetValues(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return _values.Where(pair => pair.Key.Equals(name)).Select(pair => pair.Value).ToList();
        }

        public IList<string> this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }
                return GetValues(name);
            }
        }

        public void Clear()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SessionDescription is read-only");
            }
            _values.Clear();
        }

        public int Count
        {
            get { return _values.Count(); }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        internal AttributeDictionary(SessionDescription sd)
        {
            SessionDescription = sd;
            Media = null;
        }

        internal AttributeDictionary(Media media)
        {
            SessionDescription = null;
            Media = media;
        }

        private List<KeyValuePair<string, string>> _values = new List<KeyValuePair<string, string>>();
    }
}
