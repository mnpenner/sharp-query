using System.Collections.Generic;

namespace XCSS3SE
{
    internal static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue @default = default(TValue))
        {
            return dict.ContainsKey(key) ? dict[key] : @default;
        }
    }
}