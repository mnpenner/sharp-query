using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using HtmlAgilityPlus.Extensions;

namespace HtmlAgilityPlus
{
    public partial class SharpQuery
    {
        /// <summary>
        /// Check the current matched set of elements against a selector and return true if at least one of these elements has a descendant that matches the selector.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match elements against.</param>
        /// <returns></returns>
        public bool Has(string selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check the current matched set of elements against a selector and return true if at least one of these elements has a descendant that matches the DOM element.
        /// </summary>
        /// <param name="contained">A string containing a selector expression to match elements against.</param>
        /// <returns></returns>
        public bool Has(HtmlNode contained)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check the current matched set of elements against a selector and return true if at least one of these elements matches the selector.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match elements against.</param>
        /// <returns></returns>
        public bool Is(string selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search for a given element from among the matched elements.
        /// </summary>
        /// <returns>An integer indicating the position of the first element within the SharpQuery object relative to its sibling elements.</returns>
        public int Index()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search for a given element from among the matched elements.
        /// </summary>
        /// <param name="selector">A selector representing a SharpQuery collection in which to look for an element.</param>
        /// <returns>An integer indicating the position of the original element relative to the elements matched by the selector. If the element is not found, <c>Index()</c> will return -1.</returns>
        public int Index(string selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search for a given element from among the matched elements.
        /// </summary>
        /// <param name="element">The DOM element or first element within the SharpQuery object to look for.</param>
        /// <returns>An integer indicating the position of the passed element relative to the original collection.</returns>
        public int Index(HtmlNode element)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the value of an attribute for the first element in the set of matched elements.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to get.</param>
        /// <param name="default">The default value to return if the attribute is not set.</param>
        /// <returns></returns>
        public string Attr(string attributeName, string @default = null)
        {
            return _context[0].Attr(attributeName, @default);
        }

        /// <summary>
        /// Get the HTML contents of the first element in the set of matched elements.
        /// </summary>
        /// <returns></returns>
        public string Html()
        {
            return _context[0].Html();
        }

        /// <summary>
        /// Get the combined text contents of each element in the set of matched elements, including their descendants.
        /// </summary>
        /// <returns></returns>
        public string Text()
        {
            var sb = new StringBuilder();
            foreach (var n in _context)
                sb.Append(n.Text());
            return sb.ToString();
        }
    }
}
