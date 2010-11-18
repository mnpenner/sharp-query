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
        /// Pass each element in the current matched set through a function, producing a new SharpQuery object containing the return values.
        /// </summary>
        /// <param name="callback">A delegate that will be invoked for each element in the current set.</param>
        /// <returns>A new SharpQuery object containing the return nodes. <c>null</c> nodes are omitted.</returns>
        public SharpQuery Map(Func<HtmlNode, int, HtmlNode> callback)
        {
            return new SharpQuery(_context.Select(callback).Where(n => n != null), this);
        }

        /// <summary>
        /// Pass each element in the current matched set through a function, producing a new array containing the return values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback">A delegate that will be invoked for each element in the current set.</param>
        /// <returns>An array containing the return values of the callback function.</returns>
        public T[] Map<T>(Func<HtmlNode, int, T> callback)
        {
            return _context.Select(callback).ToArray();
        }

        /// <summary>
        /// Iterate over a SharpQuery object, executing a function for each matched element.
        /// </summary>
        /// <param name="function">A function to execute for each matched element.</param>
        /// <returns>The SharpQuery object, unmodified, for chaining.</returns>
        public SharpQuery Each(Action<HtmlNode, int> function)
        {
            for (int i = 0; i < _context.Count; ++i)
                function(_context[i], i);
            return this;
        }
    }
}
