using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Data
{
    [Serializable]
    [FirestoreData]
    public class DescDamage : IEnumerable<DescDamageType>
    {
        public DescDamage()
        {
            _items = new List<DescDamageType>();
        }

        public DescDamageType this[DamageTypeEnum t]
        {
            get
            {
                return _items.FirstOrDefault(_ => _.Type == t);
            }
            set
            {
                int index = _items.FindIndex(_ => _.Type == t);
                _items[index] = value;
            }
        }

        public void Add(DescDamageType element)
        {
            _items.Add(element);
        }

        public void AddRange(IEnumerable<DescDamageType> collection)
        {
            _items.AddRange(collection);
        }

        public int FindIndex(Predicate<DescDamageType> predicate)
        {
            return _items.FindIndex(predicate);
        }

        public int FindIndex(int startIndex, int count, Predicate<DescDamageType> predicate)
        {
            return _items.FindIndex(startIndex, count, predicate);
        }

        public IEnumerator<DescDamageType> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        [FirestoreProperty]
        private List<DescDamageType> _items { get; set; }
    }
}
