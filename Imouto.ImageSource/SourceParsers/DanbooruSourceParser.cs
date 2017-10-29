using System;
using System.Linq;
using AngleSharp.Dom.Html;

namespace Imouto.ImageSource.SourceParsers
{
    public class DanbooruSourceParser : SourceParser
    {
        protected override bool HasParents(string html)
        {
            return html.Contains("This post belongs to a ");
        }

        protected override string GetOriginalUrl(IHtmlDocument doc)
        {
            var firstOrDefault = doc
                .QuerySelectorAll("#post-information > ul > li")
                ?.FirstOrDefault(x => x.InnerHtml.Contains("Size: "));

            var path = firstOrDefault
                ?.QuerySelector("a")
                ?.Attributes["href"]
                ?.Value;

            return String.IsNullOrWhiteSpace(path) 
                ? String.Empty 
                : "https://danbooru.donmai.us" + path;
        }
    }
}