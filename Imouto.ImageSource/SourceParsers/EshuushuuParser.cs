using AngleSharp.Html.Dom;

namespace Imouto.ImageSource.SourceParsers
{
    class EshuushuuParser : SourceParser
    {
        protected override bool HasParents(string html)
        {
            return false;
        }

        protected override string GetOriginalUrl(IHtmlDocument doc)
        {
            return $"http://e-shuushuu.net{doc.QuerySelector(".thumb_image").Attributes["href"].Value}";
        }
    }
}
