using System;
using System.Collections.Generic;

namespace Imouto.ImageSource
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!ExtractParameters(args, out string inputFolder, out string outputFolder, out bool parentsOnly))
            {
                return;
            }

            var imageService = new ImageService(inputFolder, outputFolder, parentsOnly);
            imageService.SearchImages().Wait();
        }

        private static bool ExtractParameters(string[] args, out string inputFolder, out string outputFolder, out bool parentsOnly)
        {
            var sif = AppContext.BaseDirectory;
            var sof = AppContext.BaseDirectory;
            var spo = true;

            var p = new OptionSet()
            {
                {
                    "i|input=",
                    "input folder.",
                    inf => sif = inf
                },
                {
                    "o|output=",
                    "the {NAME} of someone to greet.",
                    outf => sof = outf
                },
                {
                    "p|parentsObly",
                    "increase debug message verbosity",
                    v =>
                    {
                        spo = v != null;
                    }
                }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);

                inputFolder = sif;
                outputFolder = sof;
                parentsOnly = spo;

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

                return false;
            }
        }
    }
}