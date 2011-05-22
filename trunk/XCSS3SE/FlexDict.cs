using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XCSS3SE
{
    internal class FlexDict : IEnumerable<KeyValuePair<string, string>>
    {
        private Dictionary<string, string> _dict = new Dictionary<string, string>();
        private static readonly Regex _re = new Regex(@"{([_a-z][_a-z0-9]*)}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public void Add(string key, string pattern)
        {
            _dict[key] = pattern;
        }

        public string Expand(string pattern)
        {
            pattern = _re.Replace(pattern, match =>
            {
                string key = match.Groups[1].Value;

                if (_dict.ContainsKey(key))
                    return "(?<"+key+">" + Expand(_dict[key]) + ")";

                return match.Value;
            });

            return pattern;
        }

        public string this[string key]
        {
            get { return Expand(_dict[key]); }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var p in _dict)
                yield return new KeyValuePair<string, string>(p.Key, this[p.Key]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
