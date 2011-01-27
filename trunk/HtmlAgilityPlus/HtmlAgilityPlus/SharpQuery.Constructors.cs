using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace HtmlAgilityPlus
{
    public partial class SharpQuery
    {
        /// <summary>
        /// Fixes some bugs in HtmlAgilityPack.
        /// </summary>
        static SharpQuery()
        {
            HtmlNode.ElementsFlags.Remove("form"); // http://htmlagilitypack.codeplex.com/workitem/23074
            HtmlNode.ElementsFlags.Add("li", HtmlElementFlag.Empty | HtmlElementFlag.Closed); // http://htmlagilitypack.codeplex.com/workitem/29218
        }

        /// <summary>
        /// Constructs a new SharpQuery object containing only the document node.
        /// </summary>
        /// <param name="htmlOrUri">A string containing HTML or an absolute URI</param>
        public SharpQuery(string htmlOrUri)
        {
            if (htmlOrUri == null) throw new ArgumentNullException("htmlOrUri");

            if (Uri.IsWellFormedUriString(htmlOrUri, UriKind.Absolute))
                LoadUri(new Uri(htmlOrUri));
            else
                LoadHtml(htmlOrUri);
        }

        /// <summary>
        /// Downloads the document at the specified URI and constructs a new SharpQuery object containing only the document node.
        /// </summary>
        /// <param name="uri">The URI of the web page or file to download.</param>
        public SharpQuery(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");
            LoadUri(uri);
        }

        /// <summary>
        /// Constructs a new SharpQuery object from the specified document containing only the document node.
        /// </summary>
        /// <param name="doc"></param>
        public SharpQuery(HtmlDocument doc)
        {
            _context.Add(doc.DocumentNode);
        }

        /// <summary>
        /// Constructs a new SharpQuery object from the specified stream containing only the document node.
        /// </summary>
        /// <param name="stream"></param>
        public SharpQuery(Stream stream)
        {
            var doc = new HtmlDocument();
            doc.Load(stream);
            _context.Add(doc.DocumentNode);
        }

        /// <summary>
        /// Constructs a new SharpQuery object containing the input node. Optionally sets a previous SharpQuery object for chaining.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="previous"></param>
        public SharpQuery(HtmlNode node, SharpQuery previous = null)
        {
            if (node == null) throw new ArgumentNullException("node");
            _previous = previous;
            _context = new List<HtmlNode>(1) { node };
        }

        /// <summary>
        /// Constructs a new SharpQuery object from the specified nodes. Optionally sets a previous SharpQuery object for chaining.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="previous"></param>
        public SharpQuery(IEnumerable<HtmlNode> nodes, SharpQuery previous = null)
        {
            if (nodes == null) throw new ArgumentNullException("nodes");
            _previous = previous;
            _context = new List<HtmlNode>(nodes);
        }

        private void LoadHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            _context.Add(doc.DocumentNode);
        }

        private void LoadUri(Uri uri)
        {
            if (!SupportedSchemes.Contains(uri.Scheme))
                throw new NotSupportedException(string.Format("'{0}' scheme not supported.", uri.Scheme));

            var doc = new HtmlDocument();
            var wc = new WebClient();

            using (var s = wc.OpenRead(uri))
            {
                doc.Load(s);
            }

            _context.Add(doc.DocumentNode);
        }
    }
}
