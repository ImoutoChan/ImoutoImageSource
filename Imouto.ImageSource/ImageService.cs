using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Imouto.ImageSource.Exceptions;
using Imouto.ImageSource.SourceParsers;
using IqdbApi;
using IqdbApi.Enums;
using IqdbApi.Models;

namespace Imouto.ImageSource
{
    class ImageService
    {
        private List<string> _log = new List<string>();
        private List<string> _hasParents = new List<string>();
        private List<string> _manualDownload = new List<string>();

        private readonly Dictionary<Source, byte> _sourcePriorities = new Dictionary<Source, byte>
        {
            {Source.Yandere, 0},
            {Source.Danbooru, 1},
            {Source.SankakuChannel, 2},
            {Source.Konachan, 5},
            {Source.Gelbooru, 5},
            {Source.Zerochan, 5},
            {Source.AnimePictures, 5},
            {Source.Eshuushuu, 5},
            {Source.TheAnimeGallery, 5},
        };

        public ImageService(string sourceFolder, string destFolder, bool parentsOnly = true)
        {
            ParentsOnly = parentsOnly;

            SourceDirectory = new DirectoryInfo(sourceFolder);
            DestFolder = new DirectoryInfo(destFolder);

            Images = GetImages().ToList();
        }

        public DirectoryInfo SourceDirectory { get; }

        public DirectoryInfo DestFolder { get; }

        public bool ParentsOnly { get; }

        public List<FileInfo> Images { get; }

        public async Task SearchImages()
        {
            var total = Images.Count;
            var counter = 0;
            foreach (var image in Images)
            {
                try
                {
                    await ProcessImage(image);
                }
                catch (Exception e)
                {
                    _log.Add($"ERR : {image.FullName} : {e.Message}");
                }
                finally
                {
                    Console.WriteLine($"Progress: {++counter * 100.0 / total :0.0}");
                }
                  
            }

            await Save(_log, "log.txt");
            await Save(_hasParents, "hasParents.txt");
            await Save(_manualDownload, "manualDownload.txt");
        }

        private async Task Save(List<string> log, string filename)
        {
            using (var sw = new StreamWriter(new FileStream(Path.Combine(DestFolder.FullName, filename), FileMode.OpenOrCreate)))
            {
                await sw.WriteLineAsync();
                await sw.WriteLineAsync();
                await sw.WriteLineAsync($"NEW SESSION {DateTimeOffset.Now}");
                await sw.WriteLineAsync();

                foreach (var line in log)
                {
                    await sw.WriteLineAsync(line);
                }
            }
        }

        private IEnumerable<FileInfo> GetImages()
        {
            if (!SourceDirectory.Exists)
            {
                throw new ArgumentException("SourceDirectory doesn't exist.");
            }

            IEnumerable<FileInfo> files = Enumerable.Empty<FileInfo>();
            foreach (string ext in new [] { "*.jpg", "*.png ", "*.jpeg" })
            {
                files = files.Concat(SourceDirectory.GetFiles(ext));
            }
            return files;
        }

        private async Task ProcessImage(FileInfo image)
        {
            var searchResult = await SearchImage(image);

            if (!searchResult.IsFound)
            {
                _log.Add($"NFD : {image.FullName}");

                await Move(image, MoveTo.NotFound);
                return;
            }

            var goodMatches = searchResult
                .Matches
                .Where(x => x.MatchType == MatchType.Best || x.MatchType == MatchType.Additional)
                .OrderBy(x => _sourcePriorities[x.Source])
                .ThenByDescending(x => x.Similarity);

            var original = goodMatches.FirstOrDefault();

            await ParseOriginal(image, original);
        }

        private async Task ParseOriginal(FileInfo fileInfo, Match original)
        {
            var sourceParser = SourceParserCreator.GetSourceParser(original.Source);

            byte[] image;
            string name;
            try
            { 
                var res = await sourceParser.Parse(original.Url, ParentsOnly);
                image = res.file;
                name = res.filename;
            }
            catch (HasParentsException e)
            {
                _log.Add($"ERROR: HASPARRENT : {original.Url}");
                _hasParents.Add(original.Url.StartsWith("//") ? "http:" + original.Url : original.Url);
                await Move(fileInfo, MoveTo.HasParrent);
                return;
            }
            catch (OriginalUrlNotFoundException e)
            {
                // Gold account is required (loli or banned artist - danbooru) or image was removed (yande.re)

                _log.Add($"ERROR: ORIGINALNOTFOUND : {original.Url}");
                _manualDownload.Add(original.Url.StartsWith("//") ? "http:" + original.Url : original.Url);
                await Move(fileInfo, MoveTo.OrigNotFound);
                return;
            }
            catch (Exception e)
            {
                _log.Add($"ERROR: UNKNOWN : {original.Url} : {e.Message}");
                await Move(fileInfo, MoveTo.Error);
                return;
            }

            var dir = Path.Combine(DestFolder.FullName, "Originals");
            CreateFolderIfNotExist(dir);

            var newFilePath = Path.Combine(dir, CleanFileName(WebUtility.UrlDecode(name)));
            if (File.Exists(newFilePath))
            {
                _log.Add($"EXISTS: {original.Url}");
                await Move(fileInfo, MoveTo.Exist);
                return;
            }

            File.WriteAllBytes(newFilePath, image);
            await Move(fileInfo, MoveTo.Found);
            _log.Add($"FOUND: {original.Url}");
        }

        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        private Task Move(FileInfo image, MoveTo moveTo, string counter = "")
        {
            return Task.Run(() =>
            {
                var newFile = new FileInfo(
                    Path.Combine(GetDestFolder(moveTo), 
                                    image.Name.Substring(0, image.Name.Length - image.Extension.Length)) 
                                        + counter 
                                        + image.Extension);

                if (newFile.Exists)
                {
                    if (image.CalculateMd5HashForFile() == newFile.CalculateMd5HashForFile())
                    {
                        image.Delete();
                        _log.Add($"NOT MOVED (already exists): {image.FullName}");
                        _log.Add("Removed");
                        return;
                    }
                    else
                    {
                        _log.Add($"NOT MOVED (exists but wrong md5): {image.FullName}");
                        return;
                    }
                }

                image.MoveTo(newFile.FullName);
            });
        }

        private string GetDestFolder(MoveTo moveTo)
        {
            string result; 

            switch (moveTo)
            {
                case MoveTo.NotFound:
                    result = Path.Combine(DestFolder.FullName, "NotFound");
                    break;
                case MoveTo.Error:
                    result = Path.Combine(DestFolder.FullName, "Error");
                    break;
                case MoveTo.Exist:
                    result = Path.Combine(DestFolder.FullName, "Exist");
                    break;
                case MoveTo.Found:
                    result = Path.Combine(DestFolder.FullName, "Found");
                    break;
                case MoveTo.HasParrent:
                    result = Path.Combine(DestFolder.FullName, "HasParrent");
                    break;
                case MoveTo.OrigNotFound:
                    result = Path.Combine(DestFolder.FullName, "OrigNotFound");
                    break;
                default:
                    result = Path.Combine(DestFolder.FullName, "Default");
                    break;
            }

            CreateFolderIfNotExist(result);

            return result;
        }

        private void CreateFolderIfNotExist(string path)
        {
            var di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                di.Create();
            }
        }

        private static async Task<SearchResult> SearchImage(FileInfo imageServiceImage)
        {
            var iqdbClient = new IqdbClient();

            using (var stream = new FileStream(imageServiceImage.FullName, FileMode.Open))
            {
                return await iqdbClient.SearchFile(stream);
            }
        }
    }
}