## May 22/2011 Update ##

Complete rewrite. It now depends on [SgmlReader](http://developer.mindtouch.com/SgmlReader) instead of **HtmlAgilityPack**. I've found HtmlAgilityPack parses some documents incorrectly and thus is unreliable. Hopefully SgmlReader is better. Elements are now returned as the native `System.Xml.XmlElement` instead, with a few extension methods.

The class only has one method now, `.Find`, which returns `IEnumerable<XmlElement>`.

You can find a copy of `SgmlReaderDll.dll` in the downloads section or download and compile the newest version from their website.

## Example ##

```
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Loading...");
        var sq = new SharpQuery("http://en.wikipedia.org/wiki/Cheese");
        Console.WriteLine("Searching...");
        foreach (var el in sq.Find("html body.mediawiki div#content div#bodyContent h2 span.mw-headline"))
        {
            Console.WriteLine("{0} > {1}", el.CssSelector(), el.InnerText);
        }

        Console.WriteLine("Done!");
        Console.ReadLine();
    }
}
```