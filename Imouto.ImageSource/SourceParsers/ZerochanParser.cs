using AngleSharp.Dom.Html;

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
            return $"{doc.QuerySelector("#large > a").Attributes["href"].Value}";
        }
    }
}