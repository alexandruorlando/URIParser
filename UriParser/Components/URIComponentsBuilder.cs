using System;

namespace UriParser.Components
{
    public class URIComponentsBuilder
    {
        private readonly string _uri;

        public URIComponentsBuilder(string uri)
        {
            _uri = uri;
        }

        public string GetScheme(string input)
        {
            return GetStringBeforeDelimitor(_uri, Delimiters.SCHEME);
        }

        public static string GetStringAfterDelimitor(string input, char delimitor)
        {
            return input.Substring(input.IndexOf(delimitor) + 1);
        }

        public string GetStringBeforeDelimitor(string input, char delimitor)
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

        public string GetPassword(string input)
        {
            return GetStringBeforeDelimitor(input, Delimiters.PASSWORD);
        }
    }
}