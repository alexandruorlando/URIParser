using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
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

        public string GetComponent(string input, URIComponent component)
        {
            switch (component)
            {
                case URIComponent.Scheme:
                    return GetScheme(input);
                case URIComponent.User:
                    return GetUser(input);
                case URIComponent.Password:
                    return GetPassword(input);
                case URIComponent.Host:
                    return GetHost(input);
                /*case URIComponent.Port:
                    return GetPort(input); */
                case URIComponent.Path:
                    return GetPath(input);
                case URIComponent.Query:
                    return GetQuery(input);
                case URIComponent.Fragment:
                    return GetFragment(input);
                default:
                    return string.Empty;
            }
        }

        private string GetFragment(string input)
        {
            throw new NotImplementedException();
        }

        private string GetQuery(string input)
        {
            throw new NotImplementedException();
        }

        private string GetHost(string input)
        {
            throw new NotImplementedException();
        }


        /*public string GetPort(string input)
        {
            return GetStringBeforeDelimitor(input, Delimiters.PORT)
        }*/

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

        public bool IsQueryComponent(string restOfUri)
        {
            return restOfUri.Contains(Delimiters.QUERY.ToString());
        }
    }
}