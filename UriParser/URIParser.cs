
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
        private int _pathPosition = -1;
        private int _queryPosition = -1;
        private int _fragmentPosition = -1;
        private string _authorityInformation;
        private bool _hasAuthorityInformation = false;
        private const string _authorityPrefix = "//";

        public ParsedURI Parse(string uri)
        {
            _uri = uri;
            var result = new ParsedURI();
            _uriComponentsHelper = new URIComponentsHelper(uri);
            result.Scheme = _uriComponentsHelper.GetScheme(uri);
            _hasAuthorityInformation = HasAuthorityInformation(_uri);
            _pathPosition = GetPathPosition();
            _queryPosition = GetQueryPosition();
            _fragmentPosition = GetFragmentPosition();

            if ((_pathPosition > _queryPosition && _queryPosition != -1 ) || (_pathPosition > _fragmentPosition && _fragmentPosition != -1))
            {
                throw new ArgumentException();
            }

            if (_queryPosition > _fragmentPosition && _fragmentPosition != -1)
            {
                throw new ArgumentException();
            }

            if (_hasAuthorityInformation)
            {
                var firstPosition = GetAuthorityFirstPosition();
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
                    result.User = _uriComponentsHelper.GetUser(_authorityInformation);
                    _authorityInformation =
                        _uriComponentsHelper.GetStringAfterDelimitor(_authorityInformation, Delimiters.USER);
                    result.Password = _uriComponentsHelper.GetPassword(_authorityInformation);
                    _authorityInformation = _uriComponentsHelper.GetStringAfterDelimitor(_authorityInformation, '@');
                }

                if (HasPortInformation(_authorityInformation))
                {
                    result.Host = _uriComponentsHelper.GetStringBeforeDelimitor(_authorityInformation, ':');
                    _authorityInformation = _uriComponentsHelper.GetStringAfterDelimitor(_authorityInformation, ':');
                    result.Port = _authorityInformation;
                }
                else
                {
                    result.Host = _authorityInformation;
                }
            }

            return result;
        }

        private bool HasPortInformation(string authorityInformation)
        {
            return authorityInformation.Contains(':');
        }

        private bool HasUserInformation(string authorityInformation)
        {
            return authorityInformation.Contains('@');
        }

        private int GetAuthorityFirstPosition()
        {
            return _uri.IndexOf(Delimiters.PATH) + 2;
        }

        private int GetAuthorityLastPosition()
        {
            if (_pathPosition != -1)
            {
                return _pathPosition;
            }

            if (_queryPosition != -1)
            {
                return _queryPosition;
            }

            if (_fragmentPosition != -1)
            {
                return _fragmentPosition;
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


        private bool IsEndOfURI(string restOfUri)
        {
            var isPort = restOfUri.Contains(Delimiters.HOST_WITH_PORT);
            var isPath = restOfUri.Contains(Delimiters.PATH);
            var isQuery = restOfUri.Contains(Delimiters.QUERY);
            var isFragment = restOfUri.Contains(Delimiters.FRAGMENT);
            return !isPort && !isPath && !isQuery && !isFragment;
        }

        private bool IsPortPart(string restOfUri)
        {
            return _uriComponentsHelper.IsPortInformation(restOfUri);
        }

        private bool IsUserInformation(string restOfUri)
        {
            return _uriComponentsHelper.IsUserInformation(restOfUri);
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
