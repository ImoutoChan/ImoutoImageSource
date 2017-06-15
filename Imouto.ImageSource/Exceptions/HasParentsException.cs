using System;

namespace Imouto.ImageSource.Exceptions
{
    public class HasParentsException : Exception
    {
        public HasParentsException()
        {
        }

        public HasParentsException(string message) : base(message)
        {
        }
    }
}