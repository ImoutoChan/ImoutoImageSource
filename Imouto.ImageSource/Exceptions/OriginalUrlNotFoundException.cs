using System;

namespace Imouto.ImageSource.Exceptions
{
    public class OriginalUrlNotFoundException : Exception
    {
        public OriginalUrlNotFoundException()
        {
        }

        public OriginalUrlNotFoundException(string message) : base(message)
        {
        }
    }
}