using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Net;
using HtmlAgilityPlus.Extensions;

namespace HtmlAgilityPlus
{
    public partial class SharpQuery
    {
        private readonly List<HtmlNode> _context = new List<HtmlNode>();
        private readonly SharpQuery _previous = null;

        private static readonly string[] SupportedSchemes = new[] { Uri.UriSchemeHttp, Uri.UriSchemeHttps, Uri.UriSchemeFile };


        /// <summary>
        /// The DOM node context originally passed to SharpQuery(); if none was passed then context will likely be the document.
        /// </summary>
        public HtmlNode[] Context
        {
            get { throw new NotImplementedException(); }
        }

            private struct Combinators
        {
            public const char DirectChild = '>';
            public const char NextElement = '+';
            public const char NextSiblings = '~';
            public const char Descendant = ' ';
            public static readonly char[] AsArray = { DirectChild, NextElement, NextSiblings, Descendant };
        }

        private struct Operators
        {
            public const string PrefixEquals = "|=";
            public const string ContainsSubstring = "*=";
            public const string ContainsWord = "~=";
            public const string EndsWith = "$=";
            public const string EqualTo = "=";
            public const string NotEqualTo = "!=";
            public const string StartsWith = "^=";
            public const string GreaterThanOrEqualTo = ">=";
            public const string LessThanOrEqualTo = "<=";
            public const string GreaterThan = ">";
            public const string LessThan = "<";
            public const string AsPattern = @"[|*~$!^%<>]?=|[<>]";
        }

        private struct Selectors
        {
            public const char Id = '#';
            public const char Class = '.';
            public const char Pseudo = ':';
            public const char Attribute = '[';
            public static readonly char[] AsArray = { Id, Class, Pseudo, Attribute };
        }
    }
}
