using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imouto.ImageSource
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            if (!ExtractParameters(
                args,
                out var inputFolder,
                out var outputFolder,
                out var parentsOnly,
                out var danbooruLogin,
                out var danbooruApiKey)) return;

            var imageService = new ImageService(inputFolder, outputFolder, parentsOnly, danbooruLogin, danbooruApiKey);
            await imageService.SearchImages();
        }

        private static bool ExtractParameters(
            string[] args,
            out string inputFolder,
            out string outputFolder,
            out bool parentsOnly,
            out string danbooruLogin,
            out string danbooruApiKey)
        {
            var sif = AppContext.BaseDirectory;
            var sof = AppContext.BaseDirectory;
            var spo = true;
            
            var dl = string.Empty;
            var da = string.Empty;

            var p = new OptionSet
            {
                {
                    "i|input=",
                    "input folder",
                    inf => sif = inf
                },
                {
                    "o|output=",
                    "output folder",
                    outf => sof = outf
                },
                {
                    "p|parentsOnly",
                    "download parents only",
                    v => spo = v != null
                },
                {
                    "dl|danbooruLogin=",
                    "danbooru login",
                    x => dl = x
                },
                {
                    "da|danbooruApiKey=",
                    "danbooru api key",
                    x => da = x
                }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);

                inputFolder = sif;
                outputFolder = sof;
                parentsOnly = spo;
                danbooruLogin = dl;
                danbooruApiKey = da;

                return true;
            }
            catch (OptionException e)
            {
                Console.Write("SourceImageParser: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Invalid arguments.");

                inputFolder = null;
                outputFolder = null;
                parentsOnly = false;
                danbooruLogin = null;
                danbooruApiKey = null;

                return false;
            }
        }
    }
}
