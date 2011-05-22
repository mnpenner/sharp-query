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
<!DOCTYPE html>
<html>
    <head>
        <style>
            textarea { height:45px; }
        </style>
        <script src=""http://code.jquery.com/jquery-1.4.4.js""></script>
    </head>
    <body>
        <form>
            <input type=""checkbox"" name=""newsletter"" checked=""checked"" value=""Hourly"" />
            <input type=""checkbox"" name=""newsletter"" value=""Daily"" />
            <input type=""checkbox"" name=""newsletter"" value=""Weekly"" />
            <input type=""checkbox"" name=""newsletter"" checked=""checked"" value=""Monthly"" />
            <input type=""checkbox"" name=""newsletter"" value=""Yearly"" />

            <input type=""button"" value=""Input Button"" />
            <input type=""file"" />
            <input type=""hidden"" />
            <input type=""image"" />
            <input type=""password"" />
            <input type=""reset"" />
            <input type=""submit"" />
            <input type=""text"" />

            <input name=""email"" disabled=""disabled"" />
            <input name=""id"" />

            <input type=""radio"" name=""asdf"" />
            <input type=""radio"" name=""asdf"" />
            
            <select>
                <option>Option A</option>
                <option>Option B</option>
            </select>

            <select multiple=""multipe"">
                <option>Option 1</option>
                <option selected=""selected"">Option 2</option>
                <option selected=""selected"">Option 3</option>
            </select>

            <textarea></textarea>

            <button>Button 1</button>
            <button type=""submit"">Button 2</button>
        </form>
    </body>
</html>
");
            Console.WriteLine("---");
            foreach(var n in sq.Find("form").Children())
                Console.WriteLine(n.XPath);
            Console.WriteLine("---");
            Console.ReadLine();
        }
    }
}
