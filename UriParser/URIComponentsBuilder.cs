using System;
using System.Text.RegularExpressions;

namespace UriParser
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

        public string GetStringAfterDelimitor(string input, char delimitor)
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

        public bool IsUserInformation(string input)
        {
            return input.Contains(Delimiters.PASSWORD.ToString());
        }

        public bool IsPortInformation(string input)
        {
            return input.Contains(Delimiters.HOST_WITH_PORT.ToString());
        }

        public string GetHostWithPortProvided(string restOfUri)
        {
            return GetStringBeforeDelimitor(restOfUri, Delimiters.HOST_WITH_PORT);
        }

        public bool IsPathComponent(string input)
        {
            return input.Contains(Delimiters.PATH.ToString());
        }

        public string GetPath(string restOfUri)
        {
            return GetStringAfterDelimitor(restOfUri, Delimiters.PATH);
        }
    }
}