using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Collections;
using System.Text.RegularExpressions;

namespace SharpQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            var sq = SharpQuery.Load(new Uri("http://ca.yahoo.com/?p=us"));

            foreach (var n in sq.Find(@"a[href %= /aDvErTiSiNg/i]"))
            {
                Console.Write("<{0}", n.Name);
                foreach (var a in n.Attributes)
                    Console.Write(" {0}=\"{1}\"", a.Name, a.Value);
                Console.WriteLine(" />");
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
