using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace HtmlAgilityPlus
{
    public partial class SharpQuery : IEnumerable<HtmlNode>
    {
        public IEnumerator<HtmlNode> GetEnumerator()
        {
            return _context.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
