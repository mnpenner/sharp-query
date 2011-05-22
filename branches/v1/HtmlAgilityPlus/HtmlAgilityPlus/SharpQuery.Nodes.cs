using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace HtmlAgilityPlus
{
    public partial class SharpQuery
    {
        public int Length
        {
            get { return _context.Count; }
        }

        public HtmlNode this[int index]
        {
            get { return _context[index]; }
        }

        /// <summary>
        /// Returns an enumerable that iterates over each of the matched elements and their descendants.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HtmlNode> All()
        {
            var queue = new Queue<HtmlNode>(_context);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                yield return node;
                foreach (var next in node.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element))
                    queue.Enqueue(next);
            }
        }

        /// <summary>
        /// Retrieve the DOM elements matched by the SharpQuery object.
        /// </summary>
        /// <returns></returns>
        public HtmlNode[] Get()
        {
            return ToArray();
        }

        /// <summary>
        /// Retrieve the DOM elements matched by the SharpQuery object.
        /// </summary>
        /// <param name="index">A zero-based integer indicating which element to retrieve.</param>
        /// <returns></returns>
        public HtmlNode Get(int index)
        {
            return _context.ElementAtOrDefault(index < 0 ? _context.Count + index : index);
        }

        /// <summary>
        /// Retrieve all the DOM elements contained in the SharpQuery set, as an array.
        /// </summary>
        /// <returns></returns>
        public HtmlNode[] ToArray()
        {
            return _context.ToArray();
        }

    }
}
