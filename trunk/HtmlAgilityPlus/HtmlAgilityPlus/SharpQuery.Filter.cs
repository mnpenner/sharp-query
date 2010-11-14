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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reduce the set of matched elements to those that match the selector or pass the function's test.
        /// </summary>
        /// <param name="function">A function used as a test for each element in the set.</param>
        /// <returns></returns>
        public SharpQuery Filter(Func<int, HtmlNode, bool> function)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reduce the set of matched elements to those that match the selector or pass the function's test.
        /// </summary>
        /// <param name="element">An element to match the current set of elements against.</param>
        /// <returns></returns>
        public SharpQuery Filter(HtmlNode element)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reduce the set of matched elements to those that match the selector or pass the function's test.
        /// </summary>
        /// <param name="sharpQueryObject">An existing SharpQuery object to match the current set of elements against.</param>
        /// <returns></returns>
        public SharpQuery Filter(SharpQuery sharpQueryObject)
        {
            throw new NotImplementedException();
        }
    }
}
