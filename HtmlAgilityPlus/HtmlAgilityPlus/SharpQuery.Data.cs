using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using HtmlAgilityPlus.Extensions;

namespace HtmlAgilityPlus
{
    public partial class SharpQuery
    {
        /// <summary>
        /// Store arbitrary data associated with the matched elements.
        /// </summary>
        /// <param name="key">A string naming the piece of data to set.</param>
        /// <param name="value">The new data value.</param>
        public void Data(string key, object value)
        {
            foreach(var node in _context)
                node.Data(key, value);
        }

        /// <summary>
        /// Store arbitrary data associated with the matched elements.
        /// </summary>
        /// <param name="dict">A dictionary containing key-value pairs of data to update.</param>
        public void Data(Dictionary<string, object> dict)
        {
            foreach(var node in _context)
                node.Data(dict);
        }

        /// <summary>
        /// Returns value at named data store for the first element in the SharpQuery collection, as set by data(name, value).
        /// </summary>
        /// <param name="key">Name of the data stored.</param>
        /// <returns></returns>
        public object Data(string key)
        {
            return _context[0].Data(key);
        }

        /// <summary>
        /// Returns value at named data store for the first element in the SharpQuery collection, as set by data(name, value).
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> Data()
        {
            return _context[0].Data();
        }
    }
}
