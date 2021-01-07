using System.Linq;
using AngleSharp.Html.Dom;

namespace Imouto.ImageSource.SourceParsers
{
    public class DanbooruSourceParser : SourceParser
    {
        private readonly string _login;
        private readonly string _apiKey;

        public DanbooruSourceParser(string login, string apiKey)
        {
            _login = login;
            _apiKey = apiKey;
        }

        protected override string PrepareUrl(string url)
        {
            return url + $"?&login={_login}&api_key={_apiKey}";
        }

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

            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            if (path.Contains("donmai.us"))
                return path;

            return "https://danbooru.donmai.us" + path;
        }
    }
}