using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Collections;
using System.Xml.XPath;
using System.Text.RegularExpressions;

namespace SharpQuery
{
    using AttrDict = Dictionary<string, string>;
    using System.Diagnostics;

    public static class SharpQuery
    {
        #region Document Loaders
        public static IEnumerable<HtmlNode> Load(Uri uri)
        {
            var doc = new HtmlDocument();
            WebClient wc = new WebClient();
            try
            {
                using (var str = wc.OpenRead(uri))
                    doc.Load(str);
            }
            catch (WebException) { yield break; }
            yield return doc.DocumentNode;
        }

        public static IEnumerable<HtmlNode> Load(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            yield return doc.DocumentNode;
        }

        public static IEnumerable<HtmlNode> Load(Stream stream)
        {
            var doc = new HtmlDocument();
            doc.Load(stream);
            yield return doc.DocumentNode;
        }

        public static IEnumerable<HtmlNode> Load(params HtmlNode[] nodes)
        {
            foreach (var n in nodes)
                yield return n;
        }
        #endregion

        #region Helper Methods
        private static string BuildQuery(string prefix = "descendant::", string tag = "*", AttrDict attrs = null)
        {
            StringBuilder sb = new StringBuilder(prefix ?? "");
            sb.Append(string.IsNullOrWhiteSpace(tag) ? "*" : tag);
            if (attrs != null)
                foreach (var a in attrs)
                    if (a.Value != null)
                        sb.Append(string.Format("[@{0}='{1}']", a.Key, a.Value.Replace("'", @"\'")));
                    else
                        sb.Append(string.Format("[@{0}]", a.Key));
            return sb.ToString();
        }

        internal static IEnumerable<KeyValuePair<char?, string>> SplitUnescaped(this string input, char separator = ',')
        {
            return SplitUnescaped(input, new char[] { separator });
        }


        internal static IEnumerable<KeyValuePair<char?, string>> SplitUnescaped(this string input, char[] separators)
        {
            int startIndex = 0;
            var state = new Stack<char>();
            input = input.Trim(separators);

            for (int i = 0; i < input.Length; ++i)
            {
                char c = input[i];
                char s = state.Count > 0 ? state.Peek() : default(char);

                if (state.Count > 0 && (s == '\\' || (s == '[' && c == ']') || ((s == '"' || s == '\'') && c == s)))
                    state.Pop();
                else if (c == '\\' || c == '[' || c == '"' || c == '\'')
                    state.Push(c);
                else if (state.Count == 0 && separators.Contains(c))
                {
                    int endIndex = i;
                    while (input[i] == ' ' && separators.Contains(input[i + 1])) { ++i; }
                    yield return new KeyValuePair<char?, string>(input[i], input.Substring(startIndex, endIndex - startIndex));
                    while (input[++i] == ' ') { }
                    startIndex = i;
                }
            }

            if (state.Count != 0) throw new ArgumentException("Unbalanced expression.", "input");
            yield return new KeyValuePair<char?, string>(null, input.Substring(startIndex));
        }
        #endregion

        #region Helper Structs
        private struct Filter
        {
            public string Attribute;
            public string Operator;
            public string Value;
        }
        #endregion

        private static bool FilterAttribute(HtmlNode node, Filter filter)
        {
            var value = node.GetAttributeValue(filter.Attribute, "");
            decimal dv, df;

            switch (filter.Operator)
            {
                case "|=":
                    return Regex.IsMatch(value, "^" + Regex.Escape(filter.Value) + "($|-)");
                case "*=":
                    return value.Contains(filter.Value);
                case "~=":
                    return Regex.IsMatch(value, @"(^|\s)" + Regex.Escape(filter.Value) + @"($|\s)");
                case "$=":
                    return value.EndsWith(filter.Value);
                case "=":
                    return value.Equals(filter.Value);
                case "!=":
                    return !value.Equals(filter.Value);
                case "^=":
                    return value.StartsWith(filter.Value);
                case "%=":
                    string pattern = "";
                    RegexOptions options = RegexOptions.None;
                    if (filter.Value.Length > 2 && filter.Value[0] == '/')
                    {
                        int lastSlash = filter.Value.LastIndexOf('/');
                        pattern = filter.Value.Substring(1, lastSlash - 1);
                        string modChars = filter.Value.Substring(lastSlash + 1);
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
                    else pattern = filter.Value;
                    return Regex.IsMatch(value, pattern, options);
                case ">":
                    return decimal.TryParse(filter.Value, out df) ? decimal.TryParse(value, out dv) ? 
                        dv > df : false : string.Compare(value, filter.Value) > 0;
                case ">=":
                    return decimal.TryParse(filter.Value, out df) ? decimal.TryParse(value, out dv) ?
                        dv >= df : false : string.Compare(value, filter.Value) >= 0;
                case "<":
                    return decimal.TryParse(filter.Value, out df) ? decimal.TryParse(value, out dv) ?
                        dv < df : false : string.Compare(value, filter.Value) < 0;
                case "<=":
                    return decimal.TryParse(filter.Value, out df) ? decimal.TryParse(value, out dv) ?
                        dv <= df : false : string.Compare(value, filter.Value) <= 0;
                default:
                    return false;
            }
        }

        private static IEnumerable<HtmlNode> FilterCombinator(IEnumerable<HtmlNode> leftSeq, char? combinator, IEnumerable<HtmlNode> rightSeq)
        {
            switch (combinator)
            {
                case '>':
                    foreach (var l in leftSeq)
                        foreach (var r in rightSeq)
                            if (l.XPath == r.ParentNode.XPath)
                                yield return r;
                    break;
                case '~':
                    foreach (var l in leftSeq)
                    {
                        foreach (var r in rightSeq)
                        {
                            var n = l;
                            while ((n = n.NextSibling) != null)
                                if (n.XPath == r.XPath) yield return r;
                        }
                    }
                    break;
                case '+':
                    foreach (var l in leftSeq)
                    {
                        foreach (var r in rightSeq)
                        {
                            var n = l;
                            while ((n = n.NextSibling) != null && n.NodeType != HtmlNodeType.Element) { }
                            if (n.XPath == r.XPath)
                                yield return r;
                        }
                    }
                    break;
                case ' ':
                    foreach (var l in leftSeq)
                    {
                        foreach (var r in rightSeq)
                        {
                            var n = r;
                            while ((n = n.ParentNode) != null)
                                if (n.XPath == l.XPath)
                                    yield return r;
                        }
                    }
                    break;
                default:
                    foreach (var r in rightSeq)
                        yield return r;
                    break;
            }
        }

        

        private static IEnumerable<HtmlNode> FindSimple(this IEnumerable<HtmlNode> context, string selector)
        {
            var tagName = "*";
            var attrDict = new AttrDict();
            var filters = new List<Filter>();
            var selMatch = _parseExpr.Match(selector);

            if (selMatch.Groups["tag"].Success)
                tagName = selMatch.Groups["tag"].Value;
            foreach (Capture cap in selMatch.Groups["class"].Captures)
            {
                attrDict["class"] = null;
                filters.Add(new Filter { Attribute = "class", Operator = "~=", Value = cap.Value });
            }
            if (selMatch.Groups["id"].Success)
                attrDict["id"] = selMatch.Groups["id"].Value;
            foreach (Capture cap in selMatch.Groups["attr"].Captures)
            {
                var attrMatch = _parseAttr.Match(cap.Value);
                if (attrMatch.Success)
                {
                    attrDict[attrMatch.Groups["name"].Value] = null;
                    if (attrMatch.Groups["value"].Success)
                        filters.Add(new Filter { Attribute = attrMatch.Groups["name"].Value, Operator = attrMatch.Groups["op"].Value, Value = attrMatch.Groups["value"].Value });
                }
            }

            var query = BuildQuery(tag: tagName, attrs: attrDict);

            foreach (var contextNode in context)
            {
                var resultNodes = contextNode.SelectNodes(query);

                if (resultNodes == null)
                    continue;

                foreach (var resultNode in resultNodes)
                {
                    if (filters.All(f => FilterAttribute(resultNode, f)))
                        yield return resultNode;
                }
            }
        }

        private static IEnumerable<HtmlNode> FindRecurse(this IEnumerable<HtmlNode> context, IEnumerable<KeyValuePair<char?, string>> selectors)
        {
            var first = selectors.First();
            var rest = selectors.Skip(1);
            var nodes = context.FindSimple(first.Value);
            if (rest.Any())
                return FilterCombinator(FindRecurse(context, rest), rest.First().Key, nodes);
            else
                return nodes;
        }

        private static IEnumerable<HtmlNode> FindComplex(this IEnumerable<HtmlNode> context, string selector)
        {
            return FindRecurse(context, SplitUnescaped(selector, _combinators).Reverse());
        }

        public static IEnumerable<HtmlNode> Find(this IEnumerable<HtmlNode> context, string selector)
        {
            return SplitUnescaped(selector, ',').SelectMany(s => FindComplex(context, s.Value));
        }

        public static IEnumerable<HtmlNode> Traverse(this IEnumerable<HtmlNode> context)
        {
            foreach (var n1 in context)
            {
                yield return n1;
                foreach (var n2 in n1.ChildNodes.Traverse())
                    yield return n2;
            }
        }

        #region Constants
        // reference: http://www.w3.org/TR/REC-xml/#sec-common-syn

        private static readonly string _namePattern = @"-?[_a-zA-Z]+[_a-zA-Z0-9-]*";

        private static readonly Regex _parseAttr = new Regex(@"\[\s*
            (?<name>" + _namePattern + @")\s*
            (?:
                (?<op>[|*~$!^%<>]?=|[<>])\s*
                (?<quote>['""]?)
                    (?<value>.*?)
                (?<!\\)\k<quote>\s*
            )?
        \]", RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex _parseExpr = new Regex(@"
            (?<tag>" + _namePattern + @")?
            (?:\.(?<class>" + _namePattern + @"))*
            (?:\#(?<id>" + _namePattern + @"))*
            (?<attr>\[.*?\])*
            (?::(?<pseudo>" + _namePattern + @"))*
        ", RegexOptions.IgnorePatternWhitespace);

        private static readonly char[] _combinators = new[] { '>', '+', '~', ' ' };

        private struct Combinators
        {
            public const char DirectChild = '>';
            public const char NextAdjacent = '+';
            public const char NextSiblings = '~';
            public const char Descendant = ' ';
        }

        private struct Operators
        {
            public const string PrefixEquals = "|=";
            public const string ContainsSubstring = "*=";
            public const string ContainsWord = "~=";
            public const string EndsWith = "$=";
            public const string EqualTo = "=";
            public const string NotEqualTo = "!=";
            public const string StartsWith = "^=";
            public const string MatchesRegex = "%=";
            public const string GreaterThan = ">";
            public const string GreaterThanOrEqualTo = ">=";
            public const string LessThan = "<";
            public const string LessThanOrEqualTo = "<=";
        }
        #endregion
    }
}
