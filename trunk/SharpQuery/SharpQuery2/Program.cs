using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

namespace SharpQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            var sq = SharpQuery.Load(@"
<html>
<head>
    <title>title</title>
</head>
<body>
    <p val=-1>
    <p val=0>
    <p val=1>
    <p val=x>
</body>
</html>");

            foreach (var n in sq.Find(@"body>[val>0]"))
            {
                Console.Write("<{0}", n.Name);
                foreach (var a in n.Attributes)
                    if(a.Name != "title")
                        Console.Write(" {0}=\"{1}\"", a.Name, a.Value);
                Console.WriteLine(" />");
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
