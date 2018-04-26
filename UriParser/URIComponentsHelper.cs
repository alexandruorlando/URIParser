using System;
using System.Text.RegularExpressions;

namespace UriParser
{
    public class URIComponentsHelper
    {
        private readonly string _uri;

        public URIComponentsHelper(string uri)
        {
            _uri = uri;
        }

        public void CheckPartOfURI(URIComponent partOfURI, string value)
        {
            if (partOfURI == URIComponent.Scheme)
            {
                var match = Regex.Match(value, "^[a-zA-Z0-9+.-]+$");
                if (!match.Success)
                {
                    throw new ArgumentException($"\"{_uri}\" is not a valid URI");
                }
            }
            
        }

        public string GetScheme()
        {
            return GetStringBeforeDelimitor(_uri, Delimiters.SCHEME);
        }

        public string GetStringAfterDelimitor(string input, char delimitor)
        {
            return input.Substring(input.IndexOf(delimitor) + 1);
        }

        private string GetStringBeforeDelimitor(string input, char delimitor)
        {
            if (input.Contains(delimitor.ToString()))
            {
                return input.Substring(0, input.IndexOf(delimitor));
            }

            throw new ArgumentException($"\"{_uri}\" is not a valid URI");
        }

        public string GetUser(string input)
        {
            return GetStringBeforeDelimitor(input, Delimiters.USER);
        }

        public string GetPassowrd(string input)
        {
            return GetStringBeforeDelimitor(input, Delimiters.PASSWORD);
        }
    }
}