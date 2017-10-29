using System.Text.RegularExpressions;
using AngleSharp.Dom.Html;

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
                new Regex(@"//.*\.gelbooru\.com//images/[a-f\d]{2}/[a-f\d]{2}/[a-f\d]{32}\.(png|jpg|jpeg)", RegexOptions.IgnoreCase);
            
            var outerHtml = doc.QuerySelector("body").OuterHtml;
            var match = regex.Match(outerHtml);

            return $"http:{match}";
        }
    }
}