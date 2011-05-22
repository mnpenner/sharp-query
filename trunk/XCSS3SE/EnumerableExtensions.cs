using System.Collections.Generic;
using System.Linq;

namespace XCSS3SE
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<string> NotWS(this IEnumerable<string> e)
        {
            return e.Where(s => !string.IsNullOrWhiteSpace(s));
        }
    }
}