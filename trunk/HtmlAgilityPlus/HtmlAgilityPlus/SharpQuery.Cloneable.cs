using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlAgilityPlus
{
    public partial class SharpQuery : ICloneable
    {
        /// <summary>
        /// Create a deep copy of the set of matched elements.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new SharpQuery(_context, _previous);
        }
    }
}
