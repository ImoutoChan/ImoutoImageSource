using System;
using IqdbApi.Enums;

namespace Imouto.ImageSource.SourceParsers
{
    public static class SourceParsetCreator
    {
        public static SourceParser GetSourceParser(Source source)
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
                    return new DanbooruSourceParser();
            }
        }
    }
}