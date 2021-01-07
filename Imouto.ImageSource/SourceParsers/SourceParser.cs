using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Imouto.ImageSource.Exceptions;

namespace Imouto.ImageSource.SourceParsers
{
    public abstract class SourceParser
    {
        private HttpClient HttpClient { get; }

        protected SourceParser()
        {
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
        }

        private async Task<string> LoadString(string url)
        {
            url = PrepareUrl(url);
            var response = await HttpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        protected virtual string PrepareUrl(string url) => url;

        private async Task<byte[]> LoadBytes(string url)
        {
            var response = await HttpClient.GetAsync(url);
            return ReadFully(await response.Content.ReadAsStreamAsync());
        }

        public virtual async Task<(byte[] file, string filename)> Parse(string url, bool onlyWithoutParents = true)
        {
            if (url.StartsWith("//"))
            {
                url = "http:" + url;
            }

            var result = await LoadString(url);

            var hasParents = HasParents(result);

            if (hasParents && onlyWithoutParents)
            {
                throw new HasParentsException();
            }

            return await Download(result);
        }

        protected abstract bool HasParents(string html);
        
        private string GetName(string origUrl)
        {
            return origUrl.Split('/').Last().Split('?').First();
        }

        private async Task<(byte[] file, string filename)> Download(string html)
        {
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(html);

            var origUrl = GetOriginalUrl(doc);

            if (String.IsNullOrWhiteSpace(origUrl))
            {
                throw new OriginalUrlNotFoundException();
            }

            return (await LoadBytes(origUrl), GetName(origUrl));
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }

        protected abstract string GetOriginalUrl(IHtmlDocument doc);
    }
}