using System;
using System.Text.RegularExpressions;

namespace UriParser.Components
{
    public class ValidateUriComponents
    {
        private readonly URIParser _uriParser;

        public ValidateUriComponents(URIParser uriParser)
        {
            _uriParser = uriParser;
        }

        public void CheckOptionalComponentsValidity(string uri)
        {
            if (_uriParser.HasQuery && _uriParser.PathSymbolPosition > _uriParser.QuerySymbolPosition || _uriParser.HasFragment && _uriParser.PathSymbolPosition > _uriParser.FragmentSymbolPosition)
            {
                throw new ArgumentException($"\"{uri}\" is not a valid URI");
            }

            if (_uriParser.HasFragment && _uriParser.QuerySymbolPosition > _uriParser.FragmentSymbolPosition)
            {
                throw new ArgumentException($"\"{uri}\" is not a valid URI");
            }
        }

        public void VerifyInformation(ParsedURI result)
        {
            if (_uriParser.HasAuthority && String.IsNullOrEmpty(result.Host))
            {
                throw new ArgumentException($"\"{_uriParser.Uri}\" is not a valid URI");
            }

            if (_uriParser.HasUser)
            {
                if (String.IsNullOrEmpty(result.User) || String.IsNullOrEmpty(result.Password))
                {
                    throw new ArgumentException($"\"{_uriParser.Uri}\" is not a valid URI");
                }
            }

            if (_uriParser.HasPort)
            {
                if (String.IsNullOrEmpty(result.Port))
                {
                    throw new ArgumentException($"\"{_uriParser.Uri}\" is not a valid URI");

                }
            }

            if (_uriParser.HasPath && String.IsNullOrEmpty(result.Path))
            {
                throw new ArgumentException($"\"{_uriParser.Uri}\" is not a valid URI");
            }

            if (_uriParser.HasQuery && String.IsNullOrEmpty(result.Query))
            {
                throw new ArgumentException($"\"{_uriParser.Uri}\" is not a valid URI");
            }

            if (_uriParser.HasFragment && String.IsNullOrEmpty(result.Fragment))
            {
                throw new ArgumentException($"\"{_uriParser.Uri}\" is not a valid URI");
            }

            CheckPartOfUri(URIComponent.Scheme, result.Scheme);
        }
        public void CheckPartOfUri(URIComponent partOfUri, string value)
        {
            if (partOfUri == URIComponent.Scheme)
            {
                var match = Regex.Match(value, "^[a-zA-Z0-9+.-]+$");
                if (!match.Success)
                {
                    throw new ArgumentException($"\"{_uriParser.Uri}\" is not a valid URI");
                }
            }

        }
    }
}