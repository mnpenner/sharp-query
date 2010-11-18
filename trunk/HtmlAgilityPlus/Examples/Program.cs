using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPlus;
using HtmlAgilityPack;
using HtmlAgilityPlus.Extensions;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var sq = new SharpQuery(@"
<ul>
    <li>zero</li>
    <li>one</li>
    <li>two</li>
    <li>three</li>
</ul>");
            foreach (var n in sq.Find("li:odd"))
                Console.WriteLine(n.OuterHtml);
            Console.ReadLine();
        }
    }
}
