using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using HtmlAgilityPack;

namespace HtmlAgilityPlus.Tests
{
    [TestFixture]
    class AttributeFilters
    {
        IEnumerable<HtmlNode> sq;

        [SetUp]
        public void Init()
        {
            sq = SharpQuery.LoadHtml(@"
            <html>
            <head>
                <title>title</title>
            </head>
            <body>
                <p val=-1>
                <p val=0>
                <p val=1>
                <p val=x>
                <p val=/>
                <p val>
                <p val=' '>
                <p val='a-b'>
                <p val='c1 c2'>
                <p val='A|b'>
            </body>
            </html>");
        }

        [Test]
        public void PrefixEquals()
        {
            var r = sq.Find("[val|=a]");
            Assert.AreEqual(1, r.Count());
        }

        [Test]
        public void ContainsSubstring()
        {
            var r = sq.Find("[val*=-]");
            Assert.AreEqual(2, r.Count());
        }

        [Test]
        public void ContainsWord()
        {
            var r = sq.Find("[val~=c2]");
            Assert.AreEqual(1, r.Count());
        }

        [Test]
        public void EqualTo()
        {
            var r = sq.Find("[val=x]");
            Assert.AreEqual(1, r.Count());
        }

        [Test]
        public void StartsWith()
        {
            var r = sq.Find("[val^=a-]");
            Assert.AreEqual(1, r.Count());
        }

        [Test]
        public void RegexMatch()
        {
            var r = sq.Find(@"[val%=/a\Wb/i]");
            Assert.AreEqual(2, r.Count());
        }

        [Test]
        public void GreaterThan()
        {
            var r = sq.Find(@"[val>0]");
            Assert.AreEqual(1, r.Count());
        }

        [Test]
        public void LessThan()
        {
            var r = sq.Find(@"[val<0]");
            Assert.AreEqual(1, r.Count());
        }

        [Test]
        public void GreaterThanOrEqualTo()
        {
            var r = sq.Find(@"[val>=0]");
            Assert.AreEqual(2, r.Count());
        }

        [Test]
        public void LessThanOrEqualTo()
        {
            var r = sq.Find(@"[val<=0]");
            Assert.AreEqual(2, r.Count());
        }
    }
}
