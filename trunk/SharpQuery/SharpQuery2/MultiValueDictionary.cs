using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace HtmlAgilityPlus
{
    public class MultiValueDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, List<TValue>> _dict = new Dictionary<TKey, List<TValue>>();

        public void Add(TKey key, TValue value)
        {
            if (_dict.ContainsKey(key)) _dict[key].Add(value);
            else _dict[key] = new List<TValue> { value };
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var list in _dict)
                foreach (var value in list.Value)
                    yield return new KeyValuePair<TKey, TValue>(list.Key, value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
