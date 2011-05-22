using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace XCSS3SE
{
    internal static class PrettyPrint
    {
        public static void Write(object o, int indent = 0)
        {
            if (o is XmlElement) Write((XmlElement)o);
            else if (o is IDictionary) Write((IDictionary)o, indent);
            else if (o is IEnumerable) Write((IEnumerable)o);
            else Console.Write(o);
        }

        public static void WriteLine(object o, int indent=0)
        {
            Write(o, indent);
            Console.WriteLine();
        }

        private static void Write(XmlElement e)
        {
            var sb = new StringBuilder("<").Append(e.LocalName);
            foreach (XmlAttribute a in e.Attributes)
                sb.AppendFormat(" {0}=\"{1}\"", a.LocalName, a.Value);
            if (e.HasChildNodes) sb.Append(">...</").Append(e.LocalName).Append(">");
            else sb.Append(" />");
            Console.Write(sb);
        }

        private static void Write(IEnumerable x)
        {
            var l = x.Cast<object>();
            if (!l.Any()) Console.WriteLine("[]");
            else
            {
                Console.Write("[");
                Write(l.First());
                foreach(var e in l.Skip(1))
                {
                    Console.Write(", ");
                    Write(e);
                }
                Console.Write("]");
            }
        }

        private static void Write(IDictionary dict, int indent = 0)
        {
            Console.WriteLine("  ".Repeat(indent) + "{");
            foreach(var k in dict.Keys)
            {
                Console.Write("{0}{1} = ", "  ".Repeat(indent+1), k);
                WriteLine(dict[k], indent + 1);
            }
            Console.Write("  ".Repeat(indent) + "}");
        }

        public static string Repeat(this string value, int count)
        {
            return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
        }

        private static string S(string s)
        {
            if (s == null) return "(null)";
            if (s == "") return "(empty string)";
            if (s == " ") return "(space)";
            if (Regex.IsMatch(s, @"\s+")) return "(" + s.Length + " spaces)";
            return s;
        }
    }
}
