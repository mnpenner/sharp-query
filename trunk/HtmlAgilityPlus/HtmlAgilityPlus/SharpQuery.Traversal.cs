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
        /// Add elements to the set of matched elements.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match additional elements against.</param>
        /// <returns></returns>
        public SharpQuery Add(string selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add elements to the set of matched elements.
        /// </summary>
        /// <param name="elements">One or more elements to add to the set of matched elements.</param>
        /// <returns></returns>
        public SharpQuery Add(params HtmlNode[] elements)
        {
            return new SharpQuery(_context.Concat(elements), this);
        }

        /// <summary>
        /// Add elements to the set of matched elements.
        /// </summary>
        /// <param name="elements">One or more elements to add to the set of matched elements.</param>
        /// <returns></returns>
        public SharpQuery Add(IEnumerable<HtmlNode> elements)
        {
            return new SharpQuery(_context.Concat(elements), this);
        }

        /// <summary>
        /// Add the previous set of elements on the stack to the current set.
        /// </summary>
        /// <returns></returns>
        public SharpQuery AndSelf()
        {
            return new SharpQuery(_previous._context.Concat(_context), this);
        }

        /// <summary>
        /// Get the first ancestor element that matches the selector, beginning at the current element and progressing up through the DOM tree.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match elements against.</param>
        /// <param name="context">A DOM element within which a matching element may be found. If no context is passed in then the context of the SharpQuery set will be used instead.</param>
        /// <returns></returns>
        public SharpQuery Closest(string selector, HtmlNode context = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the children of each element in the set of matched elements, including text and comment nodes.
        /// </summary>
        /// <returns></returns>
        public SharpQuery Contents()
        {
            return new SharpQuery(_context.SelectMany(node => node.ChildNodes), this);
        }

        /// <summary>
        /// Reduce the set of matched elements to the one at the specified index.
        /// </summary>
        /// <param name="index">An integer indicating the 0-based position of the element.</param>
        /// <returns></returns>
        public SharpQuery Eq(int index)
        {
            if (index < 0) index += _context.Count;
            var nodes = index >= 0 && index < _context.Count
                           ? _context.Skip(index).Take(1)
                           : Enumerable.Empty<HtmlNode>();
            return new SharpQuery(nodes, this);
        }

        /// <summary>
        /// Get the children of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns></returns>
        public SharpQuery Children()
        {
            return new SharpQuery(_context.SelectMany(n => n.ChildNodes).Where(n => n.NodeType == HtmlNodeType.Element), this);
        }

        /// <summary>
        /// Get the children of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match elements against.</param>
        /// <returns></returns>
        public SharpQuery Children(string selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reduce the set of matched elements to the first in the set.
        /// </summary>
        /// <returns></returns>
        public SharpQuery First()
        {
            return Eq(0);
        }

        /// <summary>
        /// Reduce the set of matched elements to the final one in the set.
        /// </summary>
        /// <returns></returns>
        public SharpQuery Last()
        {
            return Eq(-1);
        }

        /// <summary>
        /// Reduce the set of matched elements to those that have a descendant that matches the selector.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match elements against.</param>
        /// <returns></returns>
        public SharpQuery Having(string selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reduce the set of matched elements to those that have a descendant that matches the DOM element.
        /// </summary>
        /// <param name="contained">A DOM element to match elements against.</param>
        /// <returns></returns>
        public SharpQuery Having(HtmlNode contained)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the immediately preceding sibling of each element in the set of matched elements.
        /// </summary>
        /// <returns></returns>
        public SharpQuery Prev()
        {
            return new SharpQuery(_context.Select(n => n.PreviousSiblingElement()).Where(n => n != null), this);
        }

        /// <summary>
        /// Get the immediately preceding sibling of each element in the set of matched elements, optionally filtered by a selector. Retrieves the preceding sibling only if it matches the selector.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match elements against.</param>
        /// <returns></returns>
        public SharpQuery Prev(string selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all preceding siblings of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match elements against.</param>
        /// <returns></returns>
        public SharpQuery PrevAll(string selector = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all preceding siblings of each element up to but not including the element matched by the selector.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to indicate where to stop matching following sibling elements.</param>
        /// <returns></returns>
        public SharpQuery PrevUntil(string selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the immediately following sibling of each element in the set of matched elements.
        /// </summary>
        /// <returns></returns>
        public SharpQuery Next()
        {
            return new SharpQuery(_context.Select(n => n.NextSiblingElement()).Where(n => n != null), this);
        }

        /// <summary>
        /// Get the immediately following sibling of each element in the set of matched elements. Retrieves the next sibling only if it matches the selector.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match elements against.</param>
        /// <returns></returns>
        public SharpQuery Next(string selector)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get all following siblings of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match elements against.</param>
        /// <returns></returns>
        public SharpQuery NextAll(string selector = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all following siblings of each element up to but not including the element matched by the selector.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to indicate where to stop matching following sibling elements.</param>
        /// <returns></returns>
        public SharpQuery NextUntil(string selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove elements from the set of matched elements.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match elements against.</param>
        /// <returns></returns>
        public SharpQuery Not(string selector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove elements from the set of matched elements.
        /// </summary>
        /// <param name="elements">One or more DOM elements to remove from the matched set.</param>
        /// <returns></returns>
        public SharpQuery Not(params HtmlNode[] elements)
        {
            return new SharpQuery(_context.Where(node => !elements.Contains(node)), this);
        }

        /// <summary>
        /// Remove elements from the set of matched elements.
        /// </summary>
        /// <param name="function">A function used as a test for each element in the set.</param>
        /// <returns></returns>
        public SharpQuery Not(Func<HtmlNode, int, bool> function)
        {
            return new SharpQuery(_context.Where((n, i) => !function(n, i)), this);
        }

        /// <summary>
        /// End the most recent filtering operation in the current chain and return the set of matched elements to its previous state.
        /// </summary>
        /// <returns></returns>
        public SharpQuery End()
        {
            return _previous;
        }

        /// <summary>
        /// Get the parent of each element in the current set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns></returns>
        public SharpQuery Parent()
        {
            return new SharpQuery(_context.Select(n => n.ParentElement()).Where(n => n != null), this);
        }

        /// <summary>
        /// Get the parent of each element in the current set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match elements against.</param>
        /// <returns></returns>
        public SharpQuery Parent(string selector)
        {
            throw new NotImplementedException();
        }

        public SharpQuery Parents()
        {
            var parents = new List<HtmlNode>();
            foreach (var node in _context)
                for (HtmlNode it = node.ParentNode; it != null; it = it.ParentNode)
                    parents.Add(it);
            return new SharpQuery(parents, this);
        }

        /// <summary>
        /// Reduce the set of matched elements to a subset specified by a range of indices.
        /// </summary>
        /// <param name="start">An integer indicating the 0-based position at which the elements begin to be selected. If negative, it indicates an offset from the end of the set. If omitted, it will start at the beginning.</param>
        /// <param name="end">An integer indicating the 0-based position at which the elements stop being selected. If negative, it indicates an offset from the end of the set. If omitted, the range continues until the end of the set.</param>
        /// <param name="step">An integer indicating how many nodes to skip before taking the next one. May be negative, but not 0.</param>
        /// <returns></returns>
        public SharpQuery Slice(int? start, int? end = null, int step = 1)
        {
            if (step == 0) throw new ArgumentException("Step cannot be zero.", "step");

            if (start == null)
            {
                if (step > 0) start = 0;
                else start = _context.Count - 1;
            }
            else if (start < 0)
            {
                if (start < -_context.Count) start = 0;
                else start += _context.Count;
            }
            else if (start > _context.Count) start = _context.Count;

            if (end == null)
            {
                if (step > 0) end = _context.Count;
                else end = -1;
            }
            else if (end < 0)
            {
                if (end < -_context.Count) end = 0;
                else end += _context.Count;
            }
            else if (end > _context.Count) end = _context.Count;

            if (start == end || start < end && step < 0 || start > end && step > 0) 
                return new SharpQuery(Enumerable.Empty<HtmlNode>(), this);

            int length = (int)(((end - start) / (float)step) + 0.5f);
            var result = new List<HtmlNode>(length);
            for (int i = (int)start, j = 0; j < length; i += step, ++j)
                result.Add(_context[i]);
            return new SharpQuery(result, this);
        }
    }
}
