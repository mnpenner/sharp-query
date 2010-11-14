using System.Collections.Generic;

namespace HtmlAgilityPlus.Extensions
{
    public static class ListExtensions
    {
        public static bool ContainsKey<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }
    }
}
