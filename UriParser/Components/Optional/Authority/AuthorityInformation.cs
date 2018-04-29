namespace UriParser.Components.Optional.Authority
{
    public class AuthorityInformation
    {
        private readonly URIParser _uriParser;
        private string _authorityInformation;
        private URIComponentsBuilder _uriComponentsBuilder;

        public AuthorityInformation(URIParser uriParser)
        {
            _uriParser = uriParser;
        }

        public static bool HasAuthorityInformation(string restOfUri)
        {
            restOfUri = URIComponentsBuilder.GetStringAfterDelimitor(restOfUri, Delimiters.SCHEME);
            if (restOfUri.Length <= 2)
            {
                return false;
            }

            var twoSlashesPart = restOfUri.Substring(0, 2);

            return twoSlashesPart.Equals(Delimiters._authorityPrefix);
        }

        private int GetAuthorityStartPosition()
        {
            return _uriParser.Uri.IndexOf(Delimiters.PATH) + 2;
        }
        public int GetAuthorityEndPosition()
        {
            if (_uriParser.HasPath)
            {
                return _uriParser.PathSymbolPosition;
            }

            if (_uriParser.HasQuery)
            {
                return _uriParser.QuerySymbolPosition;
            }

            if (_uriParser.HasFragment)
            {
                return _uriParser.FragmentSymbolPosition;
            }

            return _uriParser.Uri.Length - 1;
        }

        public void SetAuthorityInformation(string uri)
        {
            var startPosition = GetAuthorityStartPosition();
            var endPosition = GetAuthorityEndPosition();
            _uriComponentsBuilder = new URIComponentsBuilder(uri);
            if (endPosition == uri.Length - 1)
            {
                _authorityInformation = uri.Substring(startPosition);
            }
            else
            {
                _authorityInformation = uri.Substring(startPosition, endPosition - startPosition);
            }

            if (HasUserInformation())
            {
                SetUserInformation();
                _authorityInformation = URIComponentsBuilder.GetStringAfterDelimitor(_authorityInformation, Delimiters.PASSWORD);
            }

            if (HasPortInformation())
            {
                _uriParser.HasPort = true;
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
            _uriParser.result.Host = _authorityInformation;
        }

        private void SetHostHavingPort()
        {
            _uriParser.result.Host = _uriComponentsBuilder.GetStringBeforeDelimitor(_authorityInformation, ':');
        }

        private void SetPort()
        {
            _authorityInformation = URIComponentsBuilder.GetStringAfterDelimitor(_authorityInformation, ':');
            _uriParser.result.Port = _authorityInformation;
        }

        private void SetUserInformation()
        {
            _uriParser.HasUser = true;
            var userInformation = new UserInformation(_uriComponentsBuilder, _uriParser.result, _authorityInformation);
            userInformation.SetUser();
            userInformation.SetPassword();
        }

        private bool HasPortInformation()
        {
            return _authorityInformation.Contains(Delimiters.HOST_WITH_PORT.ToString());
        }

        private bool HasUserInformation()
        {
            return _authorityInformation.Contains(Delimiters.PASSWORD.ToString());
        }
    }
}
