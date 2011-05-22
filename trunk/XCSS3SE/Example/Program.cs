using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace XCSS3SE
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading...");
            var sq = new SharpQuery("http://en.wikipedia.org/wiki/Cheese");
            Console.WriteLine("Searching...");
            foreach (var el in sq.Find("html body.mediawiki div#content div#bodyContent h2 span.mw-headline"))
            {
                Console.WriteLine("{0} > {1}", el.CssSelector(), el.InnerText);
            }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}