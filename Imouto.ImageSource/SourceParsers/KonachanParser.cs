using AngleSharp.Html.Dom;

namespace Imouto.ImageSource.SourceParsers
{
    class KonachanParser : SourceParser
    {
        protected override bool HasParents(string html)
        {
            return html.Contains("This post belongs to a ");
        }

        protected override string GetOriginalUrl(IHtmlDocument doc)
        {
            return $"http:{doc.QuerySelector("#highres").Attributes["href"].Value}";
        }
    }
}
