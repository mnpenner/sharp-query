using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace HtmlAgilityPlus.Tests
{
    [TestFixture]
    class Form
    {
        SharpQuery sq;

        [SetUp]
        public void Init()
        {
            sq = new SharpQuery(@"
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
            <input type=""button"" value=""Input Button"" />
            <input type=""checkbox"" />

            <input type=""file"" />
            <input type=""hidden"" />
            <input type=""image"" />

            <input type=""password"" />
            <input type=""radio"" />
            <input type=""reset"" />

            <input type=""submit"" />
            <input type=""text"" />

            <select>
                <option>Option</option>
            </select>

            <textarea></textarea>
            <button>Button</button>
        </form>
    </body>
</html>
");
        }
    }
}
