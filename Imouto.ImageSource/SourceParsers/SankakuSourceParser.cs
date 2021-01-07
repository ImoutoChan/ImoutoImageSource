using System;
using AngleSharp.Html.Dom;

namespace Imouto.ImageSource.SourceParsers
{
    public class SankakuSourceParser : SourceParser
    {
        protected override bool HasParents(string html)
        {
            return html.Contains("This post belongs to ");
        }

        protected override string GetOriginalUrl(IHtmlDocument doc)
        {
            var path = doc.QuerySelector("#highres")
                ?.Attributes["href"]
                ?.Value;

            return String.IsNullOrWhiteSpace(path)
                ? String.Empty
                : "https:" + path;
        }
    }
}