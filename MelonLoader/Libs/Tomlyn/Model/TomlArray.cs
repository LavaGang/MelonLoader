// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license. 
// See license.txt file in the project root for full license information.
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MelonLoader.Tomlyn.Model
{
    /// <summary>
    /// Runtime representation of a TOML array
    /// </summary>
    public sealed class TomlArray : TomlObject, IEnumerable<object>, IEnumerable
    {
        private readonly TomlObject[] _items;
        public TomlArray(int capacity) : base(ObjectKind.Array) => _items = new TomlObject[capacity];

        public IEnumerator<object> GetEnumerator()
        {
            foreach (var item in _items)
                yield return ToObject(item);
        }

        public IEnumerable<TomlObject> GetTomlEnumerator() => _items;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(object item)
        {
            var toml = ToTomlObject(item);
            for (int i = 0; i < _items.Length; i++)
            {
                TomlObject obj = _items[i];
                if ((obj != null) && (obj == toml))
                    return true;
            }
            return false;
        }

        public void CopyTo(object[] array, int arrayIndex) => this[arrayIndex] = array[arrayIndex];

        public int Count => _items.Length;
        public bool IsReadOnly => false;
        public int IndexOf(object item)
        {
            var toml = ToTomlObject(item);
            for (int i = 0; i < _items.Length; i++)
            {
                TomlObject obj = _items[i];
                if ((obj != null) && (obj == toml))
                    return i;
            }
            return -1;
        }

        public void Replace(int index, object item)
        {
            var toml = ToTomlObject(item);
            _items[index] = toml;
        }

        public void RemoveAt(int index) => _items[index] = null;

        public object this[int index]
        {
            get => ToObject(_items[index]);
            set => _items[index] = ToTomlObject(value);
        }

        public TomlObject GetTomlObject(int index) => _items[index];
    }
}