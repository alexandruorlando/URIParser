using System;

namespace UriParser.Exceptions
{
    public class AuthorityParametersNotFoundException : Exception
    {
        public AuthorityParametersNotFoundException()
        {
        }

        public AuthorityParametersNotFoundException(string message) : base(message)
        {
        }
    }
}