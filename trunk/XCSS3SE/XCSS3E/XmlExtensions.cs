using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace XCSS3SE
{
    public static class XmlExtensions
    {
        public static IEnumerable<XmlElement> AllElements(this XmlElement root)
        {
            yield return root;
            foreach (var e in root.ChildNodes.OfType<XmlElement>().SelectMany(child => child.AllElements()))
                yield return e;
        }

        public static XmlElement ParentElement(this XmlElement el)
        {
            for (XmlNode it = el.ParentNode; it != null; it = it.ParentNode)
                if (it.NodeType == XmlNodeType.Element)
                    return (XmlElement)it;
            return null;
        }

        public static string[] GetClasses(this XmlElement el, string attr="class")
        {
            string str = el.GetAttribute(attr).Trim();
            return str == "" ? new string[0] : Regex.Split(str, @"\s+");
        }

        public static string CssSelector(this XmlElement el)
        {
            var sb = new StringBuilder(el.Name);
            if (el.HasAttribute("id")) sb.AppendFormat("#{0}", el.GetAttribute("id"));
            foreach (var c in el.GetClasses())
                sb.AppendFormat(".{0}", c);
            return sb.ToString();
        }

        public static string CssPath(this XmlElement el, string combinator=" ")
        {
            var selectors = new List<string>();
            do selectors.Add(el.CssSelector());
            while ((el = el.ParentElement()) != null);
            return string.Join(combinator, selectors.Reverse<string>());
        }

        public static XmlElement NextSiblingElement(this XmlElement el)
        {
            for (XmlNode it = el.NextSibling; it != null; it = it.NextSibling)
                if (it.NodeType == XmlNodeType.Element)
                    return (XmlElement)it;
            return null;
        }

        public static XmlElement PreviousSiblingElement(this XmlElement el)
        {
            for (XmlNode it = el.PreviousSibling; it != null; it = it.PreviousSibling)
                if (it.NodeType == XmlNodeType.Element)
                    return (XmlElement)it;
            return null;
        }

        public static List<XmlElement> ChildElements(this XmlElement el)
        {
            return el.ChildNodes.OfType<XmlElement>().ToList();
        }

        public static string AsString(this XmlElement el)
        {
            var sb = new StringBuilder("<").Append(el.LocalName);
            foreach(XmlAttribute a in el.Attributes)
                sb.AppendFormat(" {0}=\"{1}\"", a.LocalName, a.Value);
            sb.Append(el.HasChildNodes ? ">" : "/>");
            return sb.ToString();
        }
    }
}
