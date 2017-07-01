using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Imouto.ImageSource.Exceptions;

namespace Imouto.ImageSource.SourceParsers
{
    public abstract class SourceParser
    {
        private HttpClient HttpClient { get; }

        protected SourceParser()
        {
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            HttpClient.DefaultRequestHeaders.Add("DNT", "1");
            HttpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, sdch");
            HttpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.8,ru;q=0.6");
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
            HttpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            HttpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            HttpClient.DefaultRequestHeaders.Add("Cookie", "__cfduid=dff355a88ad647fb8fb6a5f2aa79dc8311496853266; BetterJsPop0=1; blacklisted_tags=; locale=en; __atuvc=1%7C23; __atuvs=59382b6be5df4ac2000");
        }

        private async Task<string> LoadString(string url)
        {
            var response = await HttpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

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
            var doc = parser.Parse(html);

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
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        protected abstract string GetOriginalUrl(IHtmlDocument doc);
    }
}