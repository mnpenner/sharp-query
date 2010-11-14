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
            var sq = new SharpQuery("http://api.jquery.com/category/selectors/");
            foreach (var n in sq.Find(".title-link"))
                Console.WriteLine(n.OuterHtml);
            Console.ReadLine();
        }
    }
}
