using System.Collections.Generic;
using System.Linq;

namespace HtmlAgilityPlus.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool ContainsKey<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        public static IEnumerable<KeyValuePair<int, T>> Enumerate<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select((e, i) => new KeyValuePair<int, T>(i, e));
        }
    }
}
