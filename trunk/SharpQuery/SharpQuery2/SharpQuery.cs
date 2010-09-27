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
    using AttrDict = System.Collections.Generic.Dictionary<string, string>;

    public static class SharpQuery
    {
        #region Document Loaders
        public static IEnumerable<HtmlNode> Load(Uri uri)
        {
            var doc = new HtmlDocument();
            WebClient wc = new WebClient();
            using (var str = wc.OpenRead(uri))
                doc.Load(str);
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

        private static IEnumerable<string> SplitCommas(string str)
        {
            int openBrackets = 0;
            int lastIndex = 0;

            for (int i = 0; i < str.Length; ++i)
            {
                switch (str[i])
                {
                    case '[':
                        openBrackets++;
                        break;
                    case ']':
                        openBrackets--;
                        break;
                    case ',':
                        if (openBrackets == 0)
                        {
                            yield return str.Substring(lastIndex, i - lastIndex);
                            lastIndex = i + 1;
                        }
                        break;
                }
            }
            yield return str.Substring(lastIndex);
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

        public static IEnumerable<HtmlNode> Find(this IEnumerable<HtmlNode> context, string selector)
        {
            var selectors = SplitCommas(selector).Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s));

            foreach (string select in selectors)
            {
                var tagName = "*";
                var attrDict = new AttrDict();
                var selMatch = _parseSelector.Match(select);
                var filters = new Queue<Filter>();
                string[] attrBits;

                if (selMatch.Groups["tag"].Success)
                {
                    switch (selMatch.Groups["type"].Value)
                    {
                        case "#":
                            attrDict.Add("id", selMatch.Groups["tag"].Value);
                            break;
                        case ".":
                            attrDict.Add("class", null);
                            filters.Enqueue(new Filter { Attribute = "class", Operator = "~=", Value = selMatch.Groups["tag"].Value });
                            break;
                        default:
                            tagName = selMatch.Groups["tag"].Value;
                            break;
                    }
                }

                if (selMatch.Groups["attrs"].Success)
                {
                    attrBits = _splitAttr.Split(selMatch.Groups["attrs"].Value);
                    foreach (var attrStr in attrBits)
                    {
                        var attrMatch = _parseAttr.Match(attrStr);

                        if (attrMatch.Groups["op"].Value == "=")
                        {
                            attrDict.Add(attrMatch.Groups["attr"].Value, attrMatch.Groups["value"].Value);
                        }
                        else
                        {
                            attrDict.Add(attrMatch.Groups["attr"].Value, null);
                            filters.Enqueue(new Filter { Attribute = attrMatch.Groups["attr"].Value, Operator = attrMatch.Groups["op"].Value, Value = attrMatch.Groups["value"].Value });
                        }
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
                        bool pass = true;

                        foreach (var filter in filters)
                        {
                            var value = resultNode.GetAttributeValue(filter.Attribute, "");

                            switch (filter.Operator)
                            {
                                case "|=":
                                    pass &= Regex.IsMatch(value, "^" + Regex.Escape(filter.Value) + "($|-)");
                                    break;
                                case "*=":
                                    pass &= value.Contains(filter.Value);
                                    break;
                                case "~=":
                                    pass &= Regex.IsMatch(value, @"(^|\s)" + Regex.Escape(filter.Value) + @"($|\s)");
                                    break;
                                case "$=":
                                    pass &= value.EndsWith(filter.Value);
                                    break;
                                case "!=":
                                    pass &= value != filter.Value;
                                    break;
                                case "^=":
                                    pass &= value.StartsWith(filter.Value);
                                    break;
                                case "=":
                                    pass &= value == filter.Value;
                                    break;
                            }
                        }

                        if (pass) yield return resultNode;
                    }
                }
            }
        }

        #region Regexes
        // reference: http://www.w3.org/TR/REC-xml/#sec-common-syn
        private static readonly string _validName = @"-?[_a-zA-Z]+[_a-zA-Z0-9-]*";
        private static readonly Regex _parseSelector = new Regex(@"
            (?<type>[#.])?
            (?<tag>" + _validName + @"|\*)?
            (?<attrs>\[.*\])?
        ", RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex _splitAttr = new Regex(@"(?<=\])(?=\[)");
        private static readonly Regex _parseAttr = new Regex(@"\[\s*
            (?<attr>" + _validName + @")\s*(?:
            (?<op>[|*~$!^]?=)\s*
            (?<quote>['""]?)
                (?<value>.*)
            (?<!\\)\k<quote>\s*
        )?\]", RegexOptions.IgnorePatternWhitespace);
        #endregion
    }
}
