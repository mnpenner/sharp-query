using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HtmlAgilityPlus.Extensions;

namespace HtmlAgilityPlus
{
    public partial class SharpQuery
    {
        /// <summary>
        /// Get the descendants of each element in the current set of matched elements, filtered by a selector.
        /// </summary>
        /// <param name="selector">A string containing a selector expression to match elements against.</param>
        /// <returns></returns>
        public SharpQuery Find(string selector)
        {
            return new SharpQuery(Find_Multi(Children().All(), selector), this);
        }

        /// <summary>
        /// Get the descendants of each element in the current set of matched elements, filtered by a selector.
        /// </summary>
        /// <param name="nodes">The nodes whose descendants are to be searched.</param>
        /// <param name="selector">A selector in the form <c>A, B</c></param>
        /// <returns></returns>
        private static IEnumerable<HtmlNode> Find_Multi(IEnumerable<HtmlNode> nodes, string selector)
        {
            return selector.SplitOn(',').SelectMany(p => Find_Single(nodes, p.Value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="selector">A selector in the form <c>A > B</c></param>
        /// <returns></returns>
        private static IEnumerable<HtmlNode> Find_Single(IEnumerable<HtmlNode> nodes, string selector)
        {
            return Find_Combinator(nodes, selector.SplitOn(Combinators.AsArray).Reverse());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="pairs">A key/value pair where the key is a combinator or null, and the value is a simple selector.</param>
        /// <returns></returns>
        private static IEnumerable<HtmlNode> Find_Combinator(IEnumerable<HtmlNode> nodes, IEnumerable<KeyValuePair<char?, string>> pairs)
        {
            var rest = pairs.Skip(1);
            return rest.Aggregate(Find_Simple(nodes, pairs.First().Value), (accum, pair) => Filter_Combinator(accum, pair.Key, Find_Combinator(nodes, rest)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="selector">A selector in the form <c>a#b.c[d='e']:f(g)</c></param>
        /// <returns></returns>
        private static IEnumerable<HtmlNode> Find_Simple(IEnumerable<HtmlNode> nodes, string selector)
        {
            var chunks = selector.SplitBefore(Selectors.AsArray).Where(s => !string.IsNullOrEmpty(s));
            return chunks.Aggregate(nodes, Filter_Chunk);
        }

        /// <summary>
        /// Filters a set of nodes based on the selector chunk.
        /// </summary>
        /// <param name="nodes">The set of nodes to filter</param>
        /// <param name="chunk">A subsection of a selector expression split on one of <c>#.[:</c></param>
        /// <returns>A subset of nodes</returns>
        private static IEnumerable<HtmlNode> Filter_Chunk(IEnumerable<HtmlNode> nodes, string chunk)
        {
            switch (chunk[0])
            {
                case Selectors.Id:
                    {
                        string id = chunk.Substring(1);
                        foreach (var node in nodes)
                            if (node.Id == id)
                                yield return node;
                    }
                    break;
                case Selectors.Class:
                    {
                        string @class = chunk.Substring(1);
                        foreach (var node in nodes)
                            if (node.Attributes["class"] != null &&
                                Regex.IsMatch(node.Attributes["class"].Value, @"(^|\s)" + Regex.Escape(@class) + @"($|\s)"))
                                yield return node;
                    }
                    break;
                case Selectors.Attribute:
                    {
                        string str = chunk.Slice(1, -1);
                        var match = Regex.Match(str, Operators.AsPattern);
                        
                        if (match.Success)
                        {
                            string attrName = str.Substring(0, match.Index).Trim();
                            string @operator = match.Value;
                            string value = str.Substring(match.Index + match.Length).Trim();

                            foreach (var node in nodes)
                                if (node.Attributes[attrName] != null && Evaluate(node.Attributes[attrName].Value, @operator, value))
                                    yield return node;
                        }
                        else
                        {
                            var attrName = str.Trim();
                            foreach (var node in nodes)
                                if (node.Attributes[attrName] != null)
                                    yield return node;
                        }
                    }
                    break;
                case Selectors.Pseudo:
                    {
                        var parts = chunk.Substring(1).SplitBefore('(').ToArray();
                        string funcName = parts[0];
                        var args = new List<string>();
                        if (parts.Length >= 2)
                            args.AddRange(parts[1].Slice(1,-1).SplitOn(',').Select(p => p.Value));
                        foreach (var node in Filter_Pseudo(nodes, funcName, args.ToArray()))
                            yield return node;

                    }
                    break;
                default:
                    {
                        string tagName = chunk;
                        if (tagName == "*")
                            foreach (var node in nodes)
                                yield return node;
                        else
                            foreach (var node in nodes)
                                if (node.Name == tagName)
                                    yield return node;
                    }
                    break;
            }
        }

        private static IEnumerable<HtmlNode> Filter_Pseudo(IEnumerable<HtmlNode> nodes, string funcName, IList<string> args)
        {
            switch (funcName.ToLower())
            {
                case "button":
                    foreach (var node in nodes)
                        if (node.Name == "button" || (node.Name == "input" && node.Attr("type") == "button"))
                            yield return node;
                    break;
                case "checkbox":
                    foreach (var node in nodes)
                        if (node.Name == "input" && node.Attr("type") == "checkbox")
                            yield return node;
                    break;
                case "checked":
                    foreach (var node in nodes)
                        if (node.Name == "input" && (node.Attr("type") == "checkbox" || node.Attr("type") == "radio") && node.HasAttr("checked"))
                            yield return node;
                    break;
                case "contains":
                    throw new NotImplementedException();
                    break;
                case "disabled":
                    foreach (var node in nodes)
                        if (node.HasAttr("disabled"))
                            yield return node;
                    break;
                case "empty":
                    foreach (var node in nodes)
                        if (node.ChildNodes.Count == 0)
                            yield return node;
                    break;
                case "empty-value":
                    foreach (var node in nodes)
                    {
                        var val = node.Val();
                        if ((val is string[] && val.Length == 0) || (val is string && string.IsNullOrWhiteSpace(val)))
                            yield return node;
                    }
                    break;
                case "enabled":
                    foreach (var node in nodes)
                        if (!node.HasAttr("disabled"))
                            yield return node;
                    break;
                case "eq":
                    {
                        int index = int.Parse(args[0]);
                        if (nodes.Count() >= index)
                            yield return nodes.ElementAt(index);
                    }
                    break;
                case "even":
                    foreach (var p in nodes.Enumerate())
                        if (p.Key % 2 == 0) yield return p.Value;
                    break;
                case "file":
                    foreach (var node in nodes)
                        if (node.Name == "input" && node.Attr("type") == "file")
                            yield return node;
                    break;
                case "first-child":
                    foreach (var node in nodes)
                        if (node.PreviousSibling == null)
                            yield return node;
                    break;
                case "first":
                    if (nodes.Any()) yield return nodes.First();
                    break;
                case "gt":
                    throw new NotImplementedException();
                    break;
                case "has":
                    throw new NotImplementedException();
                    break;
                case "header":
                    foreach (var node in nodes)
                        if (Regex.IsMatch(node.Name, "h[1-6]"))
                            yield return node;
                    break;
                case "hidden":
                    throw new NotSupportedException("Can't determine if element is visible without JavaScript and CSS support.");
                    break;
                case "image":
                    foreach (var node in nodes)
                        if (node.Name == "input" && node.Attr("type") == "image")
                            yield return node;
                    break;
                case "input":
                    foreach (var node in nodes)
                        if (node.Name == "input" || node.Name == "textarea" || node.Name == "select" || node.Name == "button")
                            yield return node;
                    break;
                case "last-child":
                    foreach (var node in nodes)
                        if (node.NextSibling == null)
                            yield return node;
                    break;
                case "last":
                    if (nodes.Any()) yield return nodes.Last();
                    break;
                case "lt":
                    throw new NotImplementedException();
                    break;
                case "not":
                    throw new NotImplementedException();
                    break;
                case "nth-child":
                    throw new NotImplementedException();
                    break;
                case "odd":
                    foreach (var p in nodes.Enumerate())
                        if (p.Key % 2 == 1) yield return p.Value;
                    break;
                case "only-child":
                    if (nodes.Count() == 1)
                        yield return nodes.Single();
                    break;
                case "parent":
                    foreach (var node in nodes)
                        if (node.ChildNodes.Count > 0)
                            yield return node;
                    break;
                case "password":
                    foreach (var node in nodes)
                        if (node.Name == "input" && node.Attr("type") == "password")
                            yield return node;
                    break;
                case "radio":
                    foreach (var node in nodes)
                        if (node.Name == "input" && node.Attr("type") == "radio")
                            yield return node;
                    break;
                case "reset":
                    foreach (var node in nodes)
                        if (node.Name == "input" && node.Attr("type") == "reset")
                            yield return node;
                    break;
                case "selected":
                    foreach (var node in nodes)
                        if (node.Name == "option" && node.HasAttr("selected"))
                            yield return node;
                    break;
                case "submit":
                    foreach (var node in nodes)
                        if ((node.Name == "input" || node.Name == "button") && node.Attr("type") == "submit")
                            yield return node;
                    break;
                case "text":
                    foreach (var node in nodes)
                        if (node.Name == "input" && (!node.HasAttr("type") || node.Attr("type") == "text"))
                            yield return node;
                    break;
                case "visible":
                    throw new NotSupportedException("Can't determine if element is visible without JavaScript and CSS support.");
                    break;
                default:
                    throw new NotSupportedException(string.Format("'{0}' pseudo selector not supported.", funcName));
            }
        }

        /// <summary>
        /// Compares two values using the specified operator.
        /// </summary>
        /// <param name="left">The value to the left of the operator</param>
        /// <param name="operator">A valid SharpQuery comparison operator.</param>
        /// <param name="right">The value to compare to; may be enclosed in quotes, slashes, or nothing.</param>
        /// <returns>True if the values pass the test given by the operator.</returns>
        private static bool Evaluate(string left, string @operator, string right)
        {
            if (string.IsNullOrEmpty(right)) throw new ArgumentException("String is empty.", "right");

            RegexOptions options = RegexOptions.None;
            string patRight = "", strRight = "";
            decimal decLeft = 0, decRight = 0;
            bool isDecimal = false;

            if (right.Length >= 2 && right[0] == right[right.Length - 1] && (right[0] == '"' || right[0] == '\''))
            {
                strRight = right.Slice(1, -1);
                patRight = Regex.Escape(strRight);
            }
            else if (right.Length >= 2 && right[0] == '/')
            {
                int lastSlash = right.LastIndexOf('/');
                if (lastSlash <= 0) throw new ArgumentException("Missing trailing slash.", "right");

                patRight = right.Substring(1, lastSlash - 1);
                string modChars = right.Substring(lastSlash + 1);

                foreach (var c in modChars)
                {
                    switch (c)
                    {
                        case 'i':
                            options |= RegexOptions.IgnoreCase;
                            break;
                        case 'm':
                            options |= RegexOptions.Multiline;
                            break;
                        case 's':
                            options |= RegexOptions.Singleline;
                            break;
                        case 'x':
                            options |= RegexOptions.IgnorePatternWhitespace;
                            break;
                    }
                }
            }
            else if (decimal.TryParse(right, out decRight))
            {
                if (decimal.TryParse(left, out decLeft))
                    isDecimal = true;
                else 
                    return false;
            }
            else throw new ArgumentException("Unrecognized format.", "right");


            switch (@operator)
            {
                case Operators.PrefixEquals:
                    return Regex.IsMatch(left, "^(" + patRight + ")($|-)", options);
                case Operators.ContainsSubstring:
                    return Regex.IsMatch(left, patRight, options);
                case Operators.ContainsWord:
                    return Regex.IsMatch(left, @"(^|\s)(" + patRight + @")($|\s)", options);
                case Operators.StartsWith:
                    return Regex.IsMatch(left, "^(" + patRight + ")", options);
                case Operators.EndsWith:
                    return Regex.IsMatch(left, "(" + patRight + ")$", options);
                
                case Operators.EqualTo:
                    return isDecimal ? decLeft == decRight : Regex.IsMatch(left, "^(" + patRight + ")$", options);
                case Operators.NotEqualTo:
                    return isDecimal ? decLeft != decRight : !Regex.IsMatch(left, "^(" + patRight + ")$", options);

                case Operators.GreaterThan:
                    return isDecimal ? decLeft > decRight : string.Compare(left, strRight) > 0;
                case Operators.GreaterThanOrEqualTo:
                    return isDecimal ? decLeft >= decRight : string.Compare(left, strRight) >= 0;
                case Operators.LessThan:
                    return isDecimal ? decLeft < decRight : string.Compare(left, strRight) < 0;
                case Operators.LessThanOrEqualTo:
                    return isDecimal ? decLeft <= decRight : string.Compare(left, strRight) <= 0;

                default:
                    return false;
            }
        }

        private static IEnumerable<HtmlNode> Filter_Combinator(IEnumerable<HtmlNode> nodes, char? combinator, IEnumerable<HtmlNode> otherNodes)
        {
            switch (combinator)
            {
                case Combinators.DirectChild:
                    foreach (var node in nodes)
                        foreach (var otherNode in otherNodes)
                        {
                            var parent = node.ParentElement();
                            if (parent != null && parent.XPath == otherNode.XPath)
                                yield return node;
                        }
                    break;
                case Combinators.NextSiblings:
                    foreach (var node in nodes)
                    {
                        foreach (var otherNode in otherNodes)
                        {
                            var sibling = otherNode;
                            while ((sibling = sibling.NextSiblingElement()) != null)
                                if (node.XPath == sibling.XPath)
                                    yield return node;
                        }
                    }
                    break;
                case Combinators.NextElement:
                    foreach (var node in nodes)
                    {
                        foreach (var otherNode in otherNodes)
                        {
                            var sibling = otherNode.NextSiblingElement();
                            if (sibling != null && node.XPath == sibling.XPath)
                                yield return node;
                        }
                    }
                    break;
                case Combinators.Descendant:
                    foreach (var node in nodes)
                    {
                        foreach (var otherNode in otherNodes)
                        {
                            var ancestor = node;
                            while ((ancestor = ancestor.ParentElement()) != null)
                                if (ancestor.XPath == otherNode.XPath)
                                    yield return node;
                        }
                    }
                    break;
            }
        }
    }
}
