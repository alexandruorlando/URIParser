
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace UriParser
{
    public class URIParser
    {
        private string _uri;
        private URIComponentsHelper _uriComponentsHelper;
        private bool _isHostSet = false;
        private int _pathSymbolPosition = -1;
        private int _querySymbolPosition = -1;
        private int _fragmentSymbolPosition = -1;
        private string _authorityInformation;
        private bool _hasAuthorityInformation = false;
        private bool _hasPath = false;
        private bool _hasQuery = false;
        private bool _hasFragment = false;
        private bool _hasUserInformation;
        private bool _hasPortInformation;
        private const string _authorityPrefix = "//";

        public ParsedURI Parse(string uri)
        {
            _uri = uri;
            var result = new ParsedURI();
            _uriComponentsHelper = new URIComponentsHelper(uri);
            result.Scheme = _uriComponentsHelper.GetScheme(uri);
            _hasAuthorityInformation = HasAuthorityInformation(_uri);
            _pathSymbolPosition = GetPathPosition();
            if (_pathSymbolPosition > 0)
            {
                _hasPath = true;
            }
            _querySymbolPosition = GetQueryPosition();
            if (_querySymbolPosition > 0)
            {
                _hasQuery = true;
            }
            _fragmentSymbolPosition = GetFragmentPosition();
            if (_fragmentSymbolPosition > 0)
            {
                _hasFragment = true;
            }

            if (_hasQuery && _pathSymbolPosition > _querySymbolPosition || 
                 _hasFragment &&_pathSymbolPosition > _fragmentSymbolPosition)
            {
                throw new ArgumentException();
            }

            if (_hasFragment && _querySymbolPosition > _fragmentSymbolPosition)
            {
                throw new ArgumentException();
            }

            if (_hasAuthorityInformation)
            {
                var firstPosition = GetAuthorityStartPosition();
                var lastPosition = GetAuthorityLastPosition();
                if (lastPosition == _uri.Length - 1)
                {
                    _authorityInformation = uri.Substring(firstPosition);
                }
                else
                {
                    _authorityInformation = uri.Substring(firstPosition, lastPosition - firstPosition);
                }
                if (HasUserInformation(_authorityInformation))
                {
                    _hasUserInformation = true;
                    result.User = _uriComponentsHelper.GetUser(_authorityInformation);
                    _authorityInformation =
                        _uriComponentsHelper.GetStringAfterDelimitor(_authorityInformation, Delimiters.USER);
                    result.Password = _uriComponentsHelper.GetPassword(_authorityInformation);
                    _authorityInformation = _uriComponentsHelper.GetStringAfterDelimitor(_authorityInformation, '@');
                }

                if (HasPortInformation(_authorityInformation))
                {
                    _hasPortInformation = true;
                    result.Host = _uriComponentsHelper.GetStringBeforeDelimitor(_authorityInformation, ':');
                    _authorityInformation = _uriComponentsHelper.GetStringAfterDelimitor(_authorityInformation, ':');
                    result.Port = _authorityInformation;
                }
                else
                {
                    result.Host = _authorityInformation;
                }
            }

            var lastIndex = _uri.Length - 1;
            if (_hasQuery)
            {
                lastIndex = _querySymbolPosition - 1;
                if (_hasPath)
                {
                    result.Path = _uri.Substring(_pathSymbolPosition + 1, lastIndex - _pathSymbolPosition);
                }
            }

            if (_hasFragment)
            {
                lastIndex = _fragmentSymbolPosition - 1;
                if (_hasPath && !_hasQuery)
                {
                    result.Path = _uri.Substring(_pathSymbolPosition + 1, lastIndex - _pathSymbolPosition);
                }

                if (_hasQuery)
                {
                    result.Query = _uri.Substring(_querySymbolPosition + 1, lastIndex - _querySymbolPosition);
                }

                lastIndex = _uri.Length - 1;
                result.Fragment = _uri.Substring(_fragmentSymbolPosition + 1, lastIndex - _fragmentSymbolPosition);
            }

            lastIndex = _uri.Length - 1;
            if (_hasPath && result.Path == null)
            {
                result.Path = _uri.Substring(_pathSymbolPosition + 1, lastIndex - _pathSymbolPosition);
            }

            if (_hasQuery && !_hasFragment)
            {
                result.Query = _uri.Substring(_querySymbolPosition + 1, lastIndex - _querySymbolPosition);
            }

            VerifyInformation(result);
            return result;
        }

        private void VerifyInformation(ParsedURI result)
        {
            if (_hasAuthorityInformation && String.IsNullOrEmpty(result.Host))
            {
                throw new ArgumentException($"\"{_uri}\" is not a valid URI");
            }

            if (_hasUserInformation)
            {
                if (String.IsNullOrEmpty(result.User) || String.IsNullOrEmpty(result.Password))
                {
                    throw new ArgumentException($"\"{_uri}\" is not a valid URI");
                }
            }

            if (_hasPortInformation)
            {
                if (String.IsNullOrEmpty(result.Port))
                {
                    throw new ArgumentException($"\"{_uri}\" is not a valid URI");

                }
            }

            if (_hasPath && String.IsNullOrEmpty(result.Path))
            {
                throw new ArgumentException($"\"{_uri}\" is not a valid URI");
            }

            if (_hasQuery && String.IsNullOrEmpty(result.Query))
            {
                throw new ArgumentException($"\"{_uri}\" is not a valid URI");
            }

            if (_hasFragment && String.IsNullOrEmpty(result.Fragment))
            {
                throw new ArgumentException($"\"{_uri}\" is not a valid URI");
            }
            _uriComponentsHelper.CheckPartOfURI(URIComponent.Scheme, result.Scheme);
        }

        private bool HasPortInformation(string authorityInformation)
        {
            return authorityInformation.Contains(':');
        }

        private bool HasUserInformation(string authorityInformation)
        {
            return authorityInformation.Contains('@');
        }

        private int GetAuthorityStartPosition()
        {
            return _uri.IndexOf(Delimiters.PATH) + 2;
        }

        private int GetAuthorityLastPosition()
        {
            if (_hasPath)
            {
                return _pathSymbolPosition;
            }

            if (_hasQuery)
            {
                return _querySymbolPosition;
            }

            if (_hasFragment)
            {
                return _fragmentSymbolPosition;
            }

            return _uri.Length - 1;
        }

        private int GetFragmentPosition()
        {
            return _uri.IndexOf(Delimiters.FRAGMENT);
        }

        private int GetQueryPosition()
        {
            return _uri.IndexOf(Delimiters.QUERY);
        }

        private int GetPathPosition()
        {
            if (_hasAuthorityInformation)
            {
                var uriExcludingAuthorityPrefix = _uriComponentsHelper.GetStringAfterDelimitor(_uri, '/').Substring(1);
                if (!uriExcludingAuthorityPrefix.Contains(Delimiters.PATH))
                {
                    return -1;
                }
                return uriExcludingAuthorityPrefix.IndexOf(Delimiters.PATH) +
                       (_uri.Length - uriExcludingAuthorityPrefix.Length);
            }

            return _uri.IndexOf(Delimiters.PATH);
        }


        private bool HasAuthorityInformation(string restOfUri)
        {
            restOfUri = _uriComponentsHelper.GetStringAfterDelimitor(restOfUri, Delimiters.SCHEME);
            if (restOfUri.Length <= 2)
            {
                return false;
            }
            
            var twoSlashesPart = restOfUri.Substring(0, 2);

            return twoSlashesPart.Equals(_authorityPrefix);
        }
    }
}
