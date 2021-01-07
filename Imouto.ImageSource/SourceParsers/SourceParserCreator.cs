using System;
using IqdbApi.Enums;

namespace Imouto.ImageSource.SourceParsers
{
    public static class SourceParserCreator
    {
        public static SourceParser GetSourceParser(Source source, string login, string apiKey)
        {
            switch (source)
            {
                default:
                    throw new NotImplementedException(source.ToString());
                case Source.SankakuChannel:
                    return new SankakuSourceParser();
                case Source.Yandere:
                    return new YandereSourceParser();
                case Source.Danbooru:
                    return new DanbooruSourceParser(login, apiKey);
                case Source.Eshuushuu:
                    return new EshuushuuParser();
                case Source.Konachan:
                    return new KonachanParser();
                case Source.Gelbooru:
                    return new GelbooruParser();
                case Source.Zerochan:
                    return new ZerochanParser();
            }
        }
    }
}