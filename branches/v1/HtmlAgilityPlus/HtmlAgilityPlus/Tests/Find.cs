using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace HtmlAgilityPlus.Tests
{
    [TestFixture]
    class Find
    {
        SharpQuery sq;

        [SetUp]
        public void Init()
        {
            sq = new SharpQuery(@"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" 
	""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
	<head>
		<title>Sample Web Page</title>
		<link rel=""shortcut icon"" href=""/favicon.ico"" />
		<link href=""/css/default.css"" rel=""stylesheet"" type=""text/css"" />
		<script type=""text/javascript"" src=""http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js""></script>
	</head>
	<body>
		<div id=""page-wrap"">
			<div id=""header"">
				<h1>My Web Page</h1>
			</div>
			<div id=""content"">
				<div id=""side-nav"">
					<ul>
						<li><a href=""/home.html"">Home</a></li>
						<li><a href=""/facts.html"">Facts</a></li>
						<li><a href=""/about.html"">About</a></li>
						<li class=""last""><a href=""/contact.html"">Contact Us</a></li>
					</ul>
				</div>
				<div id=""main"">
					<h2>Lorem Ipsum</h2>
					<p>Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Pellentesque consectetur mattis ligula ac interdum. Aenean bibendum rhoncus vehicula. Cras malesuada lorem non felis commodo condimentum. Suspendisse lacus nunc, iaculis eget luctus quis, mollis sit amet mauris. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Suspendisse diam odio, viverra ac facilisis sed, lobortis sit amet odio.</p>

					<h3>Data</h3>
					<table>
						<thead>
							<tr>
								<th>Column A</th>
								<th>Column B</th>
								<th>Column c</th>
							</tr>
						</thead>
						<tbody>
							<tr>
								<th>Header 1A</th>
								<td>Data 1B</td>
								<td>Data 1C</td>
							</tr>
							<tr>
								<th>Header 2A</th>
								<td>Data 2B</td>
								<td>Data 2C</td>
							</tr>
						</tbody>
						<tfoot>
							<tr>
								<th>Total</th>
								<td colspan=""2"">100</td>
							</tr>
						</tfoot>
					</table>
				</div>
			</div>
			<div id=""footer"">
				<ul>
					<li><a href=""/terms.html"">Terms of Service</a></li>
					<li><a href=""/privacy.html"">Privacy Policy</a></li>
					<li><a href=""/help.html"">Help</a></li>
					<li><a href=""/contact.html"">Contact Us</a></li>
				</ul>
				<p id=""copyright"">Copyright &copy; My Site 2010</p>
			</div>
		</div>
	</body>
</html>
");
        }

        [Test]
        public void GreaterThan()
        {
            Assert.AreEqual(1, sq.Find("[colspan>1]").Length);
        }

        [Test]
        public void Descendent()
        {
            Assert.AreEqual(4, sq.Find("#side-nav a").Length);
        }
    }
}
