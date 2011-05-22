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
        }

        [Test]
        public void CheckedSelector()
        {
            Assert.AreEqual(2, sq.Find(":checked").Length);
        }

        [Test]
        public void TextSelector()
        {
            Assert.AreEqual(3, sq.Find(":text").Length);
        }

        [Test]
        public void SubmitSelector()
        {
            Assert.AreEqual(2, sq.Find(":submit").Length);
        }

        [Test]
        public void ResetSelector()
        {
            Assert.AreEqual(1, sq.Find(":reset").Length);
        }

        [Test]
        public void SelectedSelector()
        {
            Assert.AreEqual(2, sq.Find(":selected").Length);
        }

        [Test]
        public void RadioSelector()
        {
            Assert.AreEqual(2, sq.Find(":radio").Length);
        }

        [Test]
        public void PasswordSelector()
        {
            Assert.AreEqual(1, sq.Find(":password").Length);
        }

        [Test]
        public void InputSelector()
        {
            Assert.AreEqual(22, sq.Find(":input").Length);
        }

        [Test]
        public void ImageSelector()
        {
            Assert.AreEqual(1, sq.Find(":image").Length);
        }

        [Test]
        public void FileSelector()
        {
            Assert.AreEqual(1, sq.Find(":file").Length);
        }

        [Test]
        public void EnabledSelector()
        {
            Assert.AreEqual(16, sq.Find("input:enabled").Length);
        }

        [Test]
        public void DisabledSelector()
        {
            Assert.AreEqual(1, sq.Find("input:disabled").Length);
        }

        [Test]
        public void CheckboxSelector()
        {
            Assert.AreEqual(5, sq.Find(":checkbox").Length);
        }

        [Test]
        public void ButtonSelector()
        {
            Assert.AreEqual(3, sq.Find(":button").Length);
        }
    }
}
