
using System;
using System.Linq;

namespace UriParser
{
    public class URIParser
    {
        internal string Uri;
        private URIComponentsBuilder _uriComponentsBuilder;
        internal int PathSymbolPosition = -1;
        internal int QuerySymbolPosition = -1;
        internal int FragmentSymbolPosition = -1;
        private string _authorityInformation;
        internal bool HasAuthority;
        internal bool HasPath;
        internal bool HasQuery;
        internal bool HasFragment;
        internal bool HasUser;
        internal bool HasPort;
        private ParsedURI _result;
        private readonly ValidateUriComponents _validateUriComponents;

        public URIParser()
        {
            _validateUriComponents = new ValidateUriComponents(this);
        }
      
        public ParsedURI Parse(string uri)
        {
            Uri = uri;
            _result = new ParsedURI();
            _uriComponentsBuilder = new URIComponentsBuilder(uri);
            SetUriComponents(uri);
            return _result;
        }

        private void SetUriComponents(string uri)
        {
            IdentifyUriOptionalComponents();
            SetScheme(uri);
            if (HasAuthority)
            {
                SetAuthorityInformation(uri);
            }

            SetOtherOptionalComponents();

            _validateUriComponents.VerifyInformation(_result);
        }

        private void SetAuthorityInformation(string uri)
        {
            var startPosition = GetAuthorityStartPosition();
            var endPosition = GetAuthorityEndPosition();
            if (endPosition == Uri.Length - 1)
            {
                _authorityInformation = uri.Substring(startPosition);
            }
            else
            {
                _authorityInformation = uri.Substring(startPosition, endPosition - startPosition);
            }

            if (HasUserInformation(_authorityInformation))
            {
                SetUserInformation();
            }

            if (HasPortInformation(_authorityInformation))
            {
                HasPort = true;
                SetHostHavingPort();
                SetPort();
            }
            else
            {
                SetHostHavingNoPort();
            }
        }

        private void SetHostHavingNoPort()
        {
            _result.Host = _authorityInformation;
        }

        private void SetHostHavingPort()
        {
            _result.Host = _uriComponentsBuilder.GetStringBeforeDelimitor(_authorityInformation, ':');
        }

        private void SetPort()
        {
            _authorityInformation = _uriComponentsBuilder.GetStringAfterDelimitor(_authorityInformation, ':');
            _result.Port = _authorityInformation;
        }

        private void SetUserInformation()
        {
            HasUser = true;
            SetUser();
            SetPassword();
        }

        private void SetPassword()
        {
            _result.Password = _uriComponentsBuilder.GetPassword(_authorityInformation);
            _authorityInformation = _uriComponentsBuilder.GetStringAfterDelimitor(_authorityInformation, '@');
        }

        private void SetUser()
        {
            _result.User = _uriComponentsBuilder.GetUser(_authorityInformation);
            _authorityInformation =
                _uriComponentsBuilder.GetStringAfterDelimitor(_authorityInformation, Delimiters.USER);
        }

        private void SetOtherOptionalComponents()
        {
            if (HasQuery)
            {
                SetComponentsHavingQuery();
            }

            if (HasFragment)
            {
                SetComponentsHavingFragment();
            }

            SetOneComponentLeft();
        }

        private void SetComponentsHavingQuery()
        {
            var lastIndex = QuerySymbolPosition - 1;
            if (HasPath)
            {
                SetPathHavingQueryInformation(lastIndex);
            }
        }

        private void SetComponentsHavingFragment()
        {
            var lastIndex = FragmentSymbolPosition - 1;
            if (HasPath && !HasQuery)
            {
                SetPathHavingFragmentInformation(lastIndex);
            }

            if (HasQuery)
            {
                SetQueryHavingFragmentInformation(lastIndex);
            }

            lastIndex = Uri.Length - 1;
            SetFragment(lastIndex);
        }

        private void SetOneComponentLeft()
        {
            var lastIndex = Uri.Length - 1;
            if (HasPath && !HasQuery && !HasFragment)
            {
                SetPathWhenIsLastComponent(lastIndex);
            }

            if (HasQuery && !HasFragment)
            {
                SetQueryWhenIsLastComponent(lastIndex);
            }
        }

        private void SetQueryWhenIsLastComponent(int lastIndex)
        {
            _result.Query = Uri.Substring(QuerySymbolPosition + 1, lastIndex - QuerySymbolPosition);
        }

        private void SetPathWhenIsLastComponent(int lastIndex)
        {
            _result.Path = Uri.Substring(PathSymbolPosition + 1, lastIndex - PathSymbolPosition);
        }

        private void SetFragment(int lastIndex)
        {
            _result.Fragment = Uri.Substring(FragmentSymbolPosition + 1, lastIndex - FragmentSymbolPosition);
        }

        private void SetQueryHavingFragmentInformation(int lastIndex)
        {
            _result.Query = Uri.Substring(QuerySymbolPosition + 1, lastIndex - QuerySymbolPosition);
        }

        private void SetPathHavingFragmentInformation(int lastIndex)
        {
            _result.Path = Uri.Substring(PathSymbolPosition + 1, lastIndex - PathSymbolPosition);
        }

        private void SetPathHavingQueryInformation(int lastIndex)
        {
            _result.Path = Uri.Substring(PathSymbolPosition + 1, lastIndex - PathSymbolPosition);
        }

       

        private void SetScheme(string uri)
        {
            _result.Scheme = _uriComponentsBuilder.GetScheme(uri);
        }

        private void IdentifyUriOptionalComponents()
        {
            HasAuthority = HasAuthorityInformation(Uri);
            PathSymbolPosition = GetPathPosition();
            if (PathSymbolPosition > 0)
            {
                HasPath = true;
            }

            QuerySymbolPosition = GetQueryPosition();
            if (QuerySymbolPosition > 0)
            {
                HasQuery = true;
            }

            FragmentSymbolPosition = GetFragmentPosition();
            if (FragmentSymbolPosition > 0)
            {
                HasFragment = true;
            }

            _validateUriComponents.CheckOptionalComponentsValidity(Uri);
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
            return Uri.IndexOf(Delimiters.PATH) + 2;
        }

        private int GetAuthorityEndPosition()
        {
            if (HasPath)
            {
                return PathSymbolPosition;
            }

            if (HasQuery)
            {
                return QuerySymbolPosition;
            }

            if (HasFragment)
            {
                return FragmentSymbolPosition;
            }

            return Uri.Length - 1;
        }

        private int GetFragmentPosition()
        {
            return Uri.IndexOf(Delimiters.FRAGMENT);
        }

        private int GetQueryPosition()
        {
            return Uri.IndexOf(Delimiters.QUERY);
        }

        private int GetPathPosition()
        {
            if (HasAuthority)
            {
                var uriExcludingAuthorityPrefix = _uriComponentsBuilder.GetStringAfterDelimitor(Uri, '/').Substring(1);
                if (!uriExcludingAuthorityPrefix.Contains(Delimiters.PATH))
                {
                    return -1;
                }
                return uriExcludingAuthorityPrefix.IndexOf(Delimiters.PATH) +
                       (Uri.Length - uriExcludingAuthorityPrefix.Length);
            }

            return Uri.IndexOf(Delimiters.PATH);
        }


        private bool HasAuthorityInformation(string restOfUri)
        {
            restOfUri = _uriComponentsBuilder.GetStringAfterDelimitor(restOfUri, Delimiters.SCHEME);
            if (restOfUri.Length <= 2)
            {
                return false;
            }
            
            var twoSlashesPart = restOfUri.Substring(0, 2);

            return twoSlashesPart.Equals(Delimiters._authorityPrefix);
        }
    }
}
