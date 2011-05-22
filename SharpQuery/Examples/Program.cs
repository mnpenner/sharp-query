using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPlus;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var sq = SharpQuery.Load(new Uri("http://stackoverflow.com/"));
            var links = sq.Find("a.question-hyperlink");

            foreach(var n in links)
            {
                Console.WriteLine(n.InnerText);
            }

            Console.ReadLine();
        }
    }
}
