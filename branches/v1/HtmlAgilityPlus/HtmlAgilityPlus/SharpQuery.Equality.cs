using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlAgilityPlus
{
    public partial class SharpQuery
    {
        public override bool Equals(object obj)
        {
            return Equals(obj as SharpQuery);
        }

        public bool Equals(SharpQuery sq)
        {
            if (sq == null) return false;
            if (ReferenceEquals(this, sq)) return true;
            if (Length != sq.Length) return false;
            if (_previous != sq._previous) return false;

            for (int i = 0; i < Length; ++i)
                if (this[i].XPath != sq[i].XPath)
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (_context != null ? _context.Aggregate(0, (x, y) => x ^ y.XPath.GetHashCode()) : 0) ^ (_previous != null ? _previous.GetHashCode() : 0);
        }
    }
}
