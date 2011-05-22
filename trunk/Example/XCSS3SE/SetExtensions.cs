using System;
using System.Collections.Generic;

namespace XCSS3SE
{
    internal static class SetExtensions
    {
        public static int Filter<T>(this HashSet<T> set, Predicate<T> match)
        {
            return set.RemoveWhere(e => !match(e));
        }
    }
}