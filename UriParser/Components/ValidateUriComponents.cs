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
            if (_uriParser.HasQuery && _uriParser.PathSymbolPosition > _uriParser.QuerySymbolPosition ||
                _uriParser.HasFragment && _uriParser.PathSymbolPosition > _uriParser.FragmentSymbolPosition)
            {
                ThrowArgumentException();
            }

            if (_uriParser.HasFragment && _uriParser.QuerySymbolPosition > _uriParser.FragmentSymbolPosition)
            {
                ThrowArgumentException();
            }
        }

        public void VerifyInformation(ParsedURI result)
        {
            if (_uriParser.HasAuthority && String.IsNullOrEmpty(result.Host))
            {
                ThrowArgumentException();
            }

            if (_uriParser.HasUser)
            {
                if (String.IsNullOrEmpty(result.User) || String.IsNullOrEmpty(result.Password))
                {
                    ThrowArgumentException();
                }
            }

            if (_uriParser.HasPort)
            {
                if (String.IsNullOrEmpty(result.Port))
                {
                    ThrowArgumentException();
                }
            }

            if (_uriParser.HasPath && String.IsNullOrEmpty(result.Path))
            {
                ThrowArgumentException();
            }

            if (_uriParser.HasQuery && String.IsNullOrEmpty(result.Query))
            {
                ThrowArgumentException();
            }

            if (_uriParser.HasFragment && String.IsNullOrEmpty(result.Fragment))
            {
                ThrowArgumentException();
            }

            CheckPartOfUri(URIComponent.Scheme, result.Scheme);
            if (result.Port != null)
            {
                CheckPartOfUri(URIComponent.Port, result.Port);
            }
        }

        public void CheckPartOfUri(URIComponent partOfUri, string value)
        {
            if (partOfUri == URIComponent.Scheme)
            {
                if(!Regex.IsMatch(value, "^[a-zA-Z0-9+.-]+$"))
                { 
                    ThrowArgumentException();
                }
            }

            if (partOfUri == URIComponent.Port)
            {
                if (!Regex.IsMatch(value, @"^\d+$"))
                {
                    ThrowArgumentException();
                }
            }
        }

        private void ThrowArgumentException()
        {
            throw new ArgumentException($"\"{_uriParser.Uri}\" is not a valid URI");
        }
    }
}