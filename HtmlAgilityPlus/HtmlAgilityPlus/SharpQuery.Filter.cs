using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace HtmlAgilityPlus
{
    public partial class SharpQuery
    {
        /// <summary>
        /// Reduce the set of matched elements to those that match the selector or pass the function's test.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match the current set of elements against.</param>
        /// <returns></returns>
        public SharpQuery Filter(string selector)
        {
            return new SharpQuery(Find_Multi(_context, selector), this);
        }

        /// <summary>
        /// Reduce the set of matched elements to those that match the selector or pass the function's test.
        /// </summary>
        /// <param name="function">A function used as a test for each element in the set.</param>
        /// <returns></returns>
        public SharpQuery Filter(Func<HtmlNode, int, bool> function)
        {
            return new SharpQuery(_context.Where(function), this);
        }

        /// <summary>
        /// Reduce the set of matched elements to those that match the selector or pass the function's test.
        /// </summary>
        /// <param name="elements">An element to match the current set of elements against.</param>
        /// <returns></returns>
        public SharpQuery Filter(params HtmlNode[] elements)
        {
            return new SharpQuery(_context.Where(node => elements.Contains(node)), this);
        }

        /// <summary>
        /// Reduce the set of matched elements to those that match the selector or pass the function's test.
        /// </summary>
        /// <param name="sharpQueryObject">An existing SharpQuery object to match the current set of elements against.</param>
        /// <returns></returns>
        public SharpQuery Filter(SharpQuery sharpQueryObject)
        {
            var nodeSet = new HashSet<HtmlNode>(_context);
            var otherNodes = new HashSet<HtmlNode>(sharpQueryObject._context);
            nodeSet.IntersectWith(otherNodes);
            return new SharpQuery(nodeSet, this);
        }
    }
}
