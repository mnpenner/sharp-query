using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System;

namespace HtmlAgilityPlus.Extensions
{
    public static class HtmlNodeExtensions
    {
        private static Dictionary<HtmlNode, Dictionary<string, object>> _data = new Dictionary<HtmlNode, Dictionary<string, object>>();

        /// <summary>
        /// Gets the previous adjacent element in the DOM tree. Excludes text and comment nodes.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static HtmlNode PreviousSiblingElement(this HtmlNode node)
        {
            for (var it = node.PreviousSibling; it != null; it = it.PreviousSibling)
                if (it.NodeType == HtmlNodeType.Element)
                    return it;
            return null;
        }

        /// <summary>
        /// Gets the next adjacent element in the DOM tree. Excludes text and comment nodes.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static HtmlNode NextSiblingElement(this HtmlNode node)
        {
            for (var it = node.NextSibling; it != null; it = it.NextSibling)
                if (it.NodeType == HtmlNodeType.Element)
                    return it;
            return null;
        }

        /// <summary>
        /// Gets the sibling elements that come after the node in the DOM tree.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static HtmlNodeCollection NextSiblingElements(this HtmlNode node)
        {
            var siblings = new HtmlNodeCollection(node.ParentNode);
            for (var it = node.NextSibling; it != null; it = it.NextSibling)
                if (it.NodeType == HtmlNodeType.Element)
                    siblings.Add(it);
            return siblings;
        }

        /// <summary>
        /// Gets the sibling elements prior to the node in the DOM tree.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static HtmlNodeCollection PreviousSiblingElements(this HtmlNode node)
        {
            var siblings = new HtmlNodeCollection(node.ParentNode);
            for (var it = node.PreviousSibling; it != null; it = it.PreviousSibling)
                if (it.NodeType == HtmlNodeType.Element)
                    siblings.Add(it);
            return siblings;
        }

        /// <summary>
        /// Gets the children, excluding text and comment nodes.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static HtmlNodeCollection ChildElements(this HtmlNode node)
        {
            var children = new HtmlNodeCollection(node);
            foreach (var child in node.ChildNodes)
                if (child.NodeType == HtmlNodeType.Element)
                    children.Add(child);
            return children;
        }

        /// <summary>
        /// Gets the parent element of the node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static HtmlNode ParentElement(this HtmlNode node)
        {
            for (var it = node.ParentNode; it != null; it = it.ParentNode)
                if (it.NodeType == HtmlNodeType.Element)
                    return it;
            return null;
        }

        /// <summary>
        /// Get the value of an attribute.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static string Attr(this HtmlNode node, string attributeName, string @default = null)
        {
            return node.GetAttributeValue(attributeName, @default);
        }

        /// <summary>
        /// Determines if the node has the specified attribute.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static bool HasAttr(this HtmlNode node, string attributeName)
        {
            return node.Attributes[attributeName] != null;
        }

        /// <summary>
        /// Determines if the node has the specified CSS class.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static bool HasClass(this HtmlNode node, string className)
        {
            return node.HasAttr("class") ? Regex.IsMatch(node.Attr("class"), @"(^|\s)" + Regex.Escape(className) + @"($|\s)") : false;
        }

        /// <summary>
        /// Get the value of the node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>A <c>string</c> containing the value of the node, or a <c>string[]</c> if there are multiple values.</returns>
        public static dynamic Val(this HtmlNode node)
        {
            switch (node.Name)
            {
                case "input":
                    return node.Attr("value");
                case "option":
                    return node.Attr("value") ?? node.InnerText;
                case "textarea":
                    return node.InnerText;
                case "select":
                    if (node.HasAttr("multiple"))
                        return node.ChildElements().Where(n => n.Name == "option" && n.HasAttr("selected")).Select(n => n.Val()).ToArray();
                    foreach (var n in node.ChildElements().Where(n => n.HasAttr("selected")))
                        return n.Val();
                    break;
            }
            return null;
        }

        /// <summary>
        /// Get the value of a style property. Will only retrieve properties specified inline.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="propertyName">A CSS property.</param>
        /// <returns></returns>
        public static string Css(this HtmlNode node, string propertyName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check to see if a DOM node is within another DOM node.
        /// </summary>
        /// <param name="container">The DOM element that may contain the other element.</param>
        /// <param name="contained">The DOM node that may be contained by the other element.</param>
        /// <returns></returns>
        public static bool Contains(this HtmlNode container, HtmlNode contained)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Store arbitrary data.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key">A string naming the piece of data to set.</param>
        /// <param name="value">The new data value.</param>
        public static void Data(this HtmlNode node, string key, object value)
        {
            _data[node][key] = value;
        }

        /// <summary>
        /// Store arbitrary data.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="dict">A dictionary containing key-value pairs of data to update.</param>
        public static void Data(this HtmlNode node, Dictionary<string, object> dict)
        {
            foreach (var p in dict)
                _data[node][p.Key] = p.Value;
        }

        /// <summary>
        /// Returns value at named data store as set by data(name, value).
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key">Name of the data stored.</param>
        /// <returns></returns>
        public static object Data(this HtmlNode node, string key)
        {
            return _data[node][key];
        }

        /// <summary>
        /// Returns value at named data store as set by data(name, value).
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Dictionary<string, object> Data(this HtmlNode node)
        {
            return _data[node];
        }

        /// <summary>
        /// Get the text contents of the node, including its descendants.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string Text(this HtmlNode node)
        {
            return node.InnerText;
        }

        /// <summary>
        /// Get the HTML contents of the node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string Html(this HtmlNode node)
        {
            return node.InnerHtml;
        }
    }
}
