using HtmlAgilityPack;

namespace HtmlAgilityPlus.Extensions
{
    public static class AttributeExtensions
    {
        public static bool ContainsKey(this HtmlAttributeCollection attributes, string key)
        {
            return attributes[key] != null;
        }
    }
}
