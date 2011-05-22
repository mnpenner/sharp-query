using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Sgml;

namespace XCSS3SE
{
    public class SharpQuery
    {
        private static readonly FlexDict _patterns;
        private static readonly Regex _re;
        private static readonly string[] _schemes = new[] { Uri.UriSchemeHttp, Uri.UriSchemeHttps, Uri.UriSchemeFile };

        private Dictionary<string, XmlElement> _idDict;
        private Dictionary<string, HashSet<XmlElement>> _classDict;
        private Dictionary<string, HashSet<XmlElement>> _typeDict;
        private Dictionary<string, HashSet<XmlElement>> _attrDict;
        private XmlDocument _xmlDoc;

        static SharpQuery()
        {
            _patterns = new FlexDict
                {
                    {"GOS"/*Group of Selectors*/, @"^\s*{Selector}(\s*,\s*{Selector})*\s*$"},
                    {"Selector", @"{SOSS}{CombinatorSOSS}*{PseudoElement}?"},
                    {"CombinatorSOSS", @"\s*{Combinator}\s*{SOSS}"},
                    {"SOSS"/*Sequence of Simple Selectors*/, @"({TypeSelector}|{UniversalSelector}){SimpleSelector}*|{SimpleSelector}+"},
                    {"SimpleSelector", @"{AttributeSelector}|{ClassSelector}|{IDSelector}|{PseudoSelector}"},
                    
                    {"TypeSelector", @"{Identifier}"},
                    {"UniversalSelector", @"\*"},
                    {"AttributeSelector", @"\[\s*{Identifier}(\s*{ComparisonOperator}\s*{AttributeValue})?\s*\]"},
                    {"ClassSelector", @"\.{Identifier}"},
                    {"IDSelector", @"#{Identifier}"},
                    {"PseudoSelector", @":{Identifier}{PseudoArgs}?"},
                    {"PseudoElement", @"::{Identifier}"},

                    {"PseudoArgs", @"\(({String}|[^)])*\)"},

                    {"ComparisonOperator", @"[~^$*|]?="},
                    {"Combinator", @"[ >+~]"},
                    
                    {"Identifier", @"-?[a-zA-Z\u00A0-\uFFFF_][a-zA-Z\u00A0-\uFFFF_0-9-]*"},

                    {"AttributeValue", @"{Identifier}|{String}"},
                    {"String", @"""((?>[^\\""\r\n]|\\\r\n|\\.)*)""|'((?>[^\\'\r\n]|\\\r\n|\\.)*)'"},
                };

            _re = new Regex(_patterns["GOS"], RegexOptions.ExplicitCapture | RegexOptions.Singleline);
        }

        public SharpQuery()
        {
            _xmlDoc = new XmlDocument
                {
                    PreserveWhitespace = false,
                    XmlResolver = null
                };
        }

        public SharpQuery(XmlDocument doc)
        {
            _xmlDoc = doc;
        }

        public SharpQuery(string htmlOrUri) : this()
        {
            if (htmlOrUri == null) 
                throw new ArgumentNullException("htmlOrUri");

            if (Uri.IsWellFormedUriString(htmlOrUri, UriKind.Absolute))
                LoadUri(htmlOrUri);
            else
                LoadHtml(htmlOrUri);
        }

        private static XmlReader GetReader(string html)
        {
            return new SgmlReader
            {
                DocType = "html",
                WhitespaceHandling = WhitespaceHandling.All,
                CaseFolding = CaseFolding.ToLower,
                InputStream = new StringReader(html),
            };
        }

        public void LoadHtml(string html)
        {
            var reader = GetReader(html);
            _xmlDoc.Load(reader);
            BuildDictionaries();
        }

        public void LoadUri(string uri)
        {
            LoadUri(new Uri(uri));
        }

        public void LoadUri(Uri uri)
        {
            string html;
            if (!_schemes.Contains(uri.Scheme))
                throw new NotSupportedException(string.Format("Scheme not supported: {0}", uri.Scheme));
            using (var wc = new WebClient())
                html = wc.DownloadString(uri);
            LoadHtml(html);
        }

        private void BuildDictionaries()
        {
            _idDict = new Dictionary<string,XmlElement>();
            _classDict = new Dictionary<string, HashSet<XmlElement>>();
            _typeDict = new Dictionary<string, HashSet<XmlElement>>();
            _attrDict = new Dictionary<string, HashSet<XmlElement>>();

            foreach(var el in _xmlDoc.DocumentElement.AllElements())
            {
                if (el.GetAttribute("id").Length > 0)
                    _idDict[el.GetAttribute("id")] = el;
                
                foreach(string c in el.GetClasses())
                {
                    if(!_classDict.ContainsKey(c))
                        _classDict[c] = new HashSet<XmlElement>();
                    _classDict[c].Add(el);
                }
   
                if(!_typeDict.ContainsKey(el.LocalName))
                    _typeDict[el.LocalName] = new HashSet<XmlElement>();
                _typeDict[el.LocalName].Add(el);

                foreach(XmlAttribute a in el.Attributes)
                {
                    if (!_attrDict.ContainsKey(a.LocalName))
                        _attrDict[a.LocalName] = new HashSet<XmlElement>();
                    _attrDict[a.LocalName].Add(el);
                }
            }
        }

        private void Filter_ID(ref HashSet<XmlElement> set, string id)
        {
            if (_idDict.ContainsKey(id))
            {
                XmlElement el = _idDict[id];
                if (set == null)
                    set = new HashSet<XmlElement> { _idDict[id] };
                else
                    set.Filter(e => e == el);
            }
            else set = new HashSet<XmlElement>();
        }

        private void Filter_Class(ref HashSet<XmlElement> set, string @class)
        {
            if (_classDict.ContainsKey(@class))
            {
                if (set == null)
                    set = new HashSet<XmlElement>(_classDict[@class]);
                else
                    set.IntersectWith(_classDict[@class]);
            }
            else set = new HashSet<XmlElement>();
        }

        private void Filter_Type(ref HashSet<XmlElement> set, string type)
        {
            if (_typeDict.ContainsKey(type))
            {
                if (set == null)
                    set = new HashSet<XmlElement>(_typeDict[type]);
                else
                    set.IntersectWith(_typeDict[type]);
            }
            else set = new HashSet<XmlElement>();
        }

        private void Filter_Attribute(ref HashSet<XmlElement> set, string name, string @operator, string value)
        {
            if (_attrDict.ContainsKey(name))
            {
                if (set == null)
                    set = new HashSet<XmlElement>(_attrDict[name]);
                else
                    set.IntersectWith(_attrDict[name]);

                if (@operator != null)
                {
                    if (value == null) throw new ArgumentNullException("value", "Value cannot be null if operator is not null");

                    if (value[0] == value.Last() && (value[0] == '"' || value[0] == '\''))
                        value = Regex.Unescape(value.Slice(1, -1));

                    switch (@operator)
                    {
                        case "=":
                            set.Filter(e => e.Attributes[name].Value == value);
                            break;
                        case "~=":
                            set.Filter(e => Regex.IsMatch(e.Attributes[name].Value, @"(^|\s)" + Regex.Escape(value) + @"(\s|$)"));
                            break;
                        case "^=":
                            set.Filter(e => e.Attributes[name].Value.StartsWith(value));
                            break;
                        case "$=":
                            set.Filter(e => e.Attributes[name].Value.EndsWith(value));
                            break;
                        case "*=":
                            set.Filter(e => e.Attributes[name].Value.Contains(value));
                            break;
                        case "|=":
                            set.Filter(e => Regex.IsMatch(e.Attributes[name].Value, "^" + Regex.Escape(value) + "(-|$)"));
                            break;
                        default:
                            throw new Exception(string.Format("Bad comparison operator: {0}", @operator));
                    }
                }
            }
            else set = new HashSet<XmlElement>();
        }

        private HashSet<XmlElement> Find_SOSS(Match match, Capture capture)
        {
            HashSet<XmlElement> set = null;

            foreach (Capture idCapture in match.Groups["IDSelector"].SubcapturesOf(capture))
                Filter_ID(ref set, idCapture.Value.Substring(1));
           
            foreach (Capture classCapture in match.Groups["ClassSelector"].SubcapturesOf(capture))
                Filter_Class(ref set, classCapture.Value.Substring(1));

            foreach (Capture typeCapture in match.Groups["TypeSelector"].SubcapturesOf(capture))
                Filter_Type(ref set, typeCapture.Value);

            foreach (Capture attributeCapture in match.Groups["AttributeSelector"].SubcapturesOf(capture))
            {
                string name, op, value;
                name = match.Groups["Identifier"].SubcapturesOf(attributeCapture).First().Value;
                Capture opCap = match.Groups["ComparisonOperator"].SubcapturesOf(attributeCapture).SingleOrDefault();
                if(opCap == null)
                {
                    op = null;
                    value = null;
                } 
                else
                {
                    op = opCap.Value;
                    value = match.Groups["AttributeValue"].SubcapturesOf(attributeCapture).Single().Value;
                }
                Filter_Attribute(ref set, name, op, value);
            }

            if (set == null)
                set = new HashSet<XmlElement>(_xmlDoc.DocumentElement.AllElements());

            return set;
        }

        public IEnumerable<XmlElement> Find(string selector)
        {
            var match = _re.Match(selector);
            
            foreach (Capture selectorCapture in match.Groups["Selector"].Captures)
            {
                var selectorCaptures = match.Groups["SOSS"].SubcapturesOf(selectorCapture).Reverse().ToList();
                var combinatorCaptures = match.Groups["Combinator"].SubcapturesOf(selectorCapture).Reverse().ToList();
                var rightElements = Find_SOSS(match, selectorCaptures[0]);

                for(int i=0; i<combinatorCaptures.Count; ++i)
                {
                    var leftElements = Find_SOSS(match, selectorCaptures[i + 1]);
                    switch(combinatorCaptures[i].Value)
                    {
                        case ">": // Child Combinator
                            rightElements.Filter(e => leftElements.Contains(e.ParentElement()));
                            break;
                        case " ": // Descendant Combinator
                            rightElements.Filter(e =>
                            {
                                for (var p = e.ParentElement(); p != null; p = p.ParentElement())
                                    if (leftElements.Contains(p))
                                        return true;
                                return false;
                            });
                            break;
                        case "+": // Adjacent Sibling Combinator
                            rightElements.Filter(e => leftElements.Contains(e.PreviousSiblingElement()));
                            break;
                        case "~": // General Sibling Combinator
                            rightElements.Filter(e =>
                            {
                                for (var p = e.PreviousSiblingElement(); p != null; p = p.PreviousSiblingElement())
                                    if (leftElements.Contains(p))
                                        return true;
                                return false;
                            });
                            break;
                    }
                }

                foreach (var e in rightElements)
                    yield return e;
            }
        }
    }
}
