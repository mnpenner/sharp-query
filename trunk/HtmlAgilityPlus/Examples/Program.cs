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
    <input type=""button"" value=""Input Button""/>
    <input type=""checkbox"" />

    <input type=""file"" />
    <input type=""hidden"" />
    <input type=""image"" />

    <input type=""password"" />
    <input type=""radio"" />
    <input type=""reset"" />

    <input type=""submit"" />
    <input type=""text"" />
    <!-- <select><option>Option<option/></select> -->

    <select multiple=""multiple"">
        <option selected=""selected"">one</option>
        <option selected=""selected"" value=""2"">two</option>
    </select>

    <textarea></textarea>
    <button>Button</button>
  </form>
  <div>
  </div>
</body>
</html>
");
            foreach (var node in sq.Find("input:odd"))
                Console.WriteLine(node.OuterHtml);
            Console.ReadLine();
        }
    }
}
