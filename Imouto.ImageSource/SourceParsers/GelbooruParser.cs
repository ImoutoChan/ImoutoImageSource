using System.Text.RegularExpressions;
using AngleSharp.Html.Dom;

namespace Imouto.ImageSource.SourceParsers
{
    class GelbooruParser : SourceParser
    {
        protected override bool HasParents(string html)
        {
            return false;
        }

        protected override string GetOriginalUrl(IHtmlDocument doc)
        {
            var regex =
                new Regex(@"//.*\.gelbooru\.com/images/[a-f\d]{2}/[a-f\d]{2}/[a-f\d]{32}\.(png|jpg|jpeg)", RegexOptions.IgnoreCase);
            
            var outerHtml = doc.QuerySelector("body").OuterHtml;
            var match = regex.Match(outerHtml);

            if (string.IsNullOrWhiteSpace(match.ToString()))
                return string.Empty;

            return $"http:{match}";
        }
    }
}