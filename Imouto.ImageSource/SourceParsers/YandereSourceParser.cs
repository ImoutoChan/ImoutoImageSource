using AngleSharp.Dom.Html;

namespace Imouto.ImageSource.SourceParsers
{
    public class YandereSourceParser : SourceParser
    {
        protected override bool HasParents(string html)
        {
            return html.Contains("This post belongs to a ");
        }

        protected override string GetOriginalUrl(IHtmlDocument doc)
        {
            return doc.QuerySelector("#highres")?.Attributes["href"]?.Value;
        }
    }
}