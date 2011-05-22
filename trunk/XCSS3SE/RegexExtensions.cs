using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XCSS3SE
{
    internal static class RegexExtensions
    {
        public static IEnumerable<Capture> SubcapturesOf(this System.Text.RegularExpressions.Group g, Capture c)
        {
            return g.Captures.Cast<Capture>().Where(x => x.Index >= c.Index && x.Index + x.Length <= c.Index + c.Length);
        }
    }
}