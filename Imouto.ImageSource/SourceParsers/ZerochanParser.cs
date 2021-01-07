using AngleSharp.Html.Dom;

namespace Imouto.ImageSource.SourceParsers
{
    class ZerochanParser : SourceParser
    {
        protected override bool HasParents(string html)
        {
            return false;
        }

        protected override string GetOriginalUrl(IHtmlDocument doc)
        {
            var full = doc.QuerySelector("#large > a.preview")?.Attributes["href"]?.Value;
            full ??= doc.QuerySelector("#large > img")?.Attributes["src"]?.Value;

            return $"{full}";
        }
    }
}