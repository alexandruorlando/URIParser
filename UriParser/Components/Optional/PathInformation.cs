using System.Linq;

namespace UriParser.Components.Optional
{
    public class PathInformation
    {
        private readonly ParsedURI _result;
        private readonly int _pathSymbolPosition;
        public bool HasPath;

        public PathInformation(ParsedURI result, int pathSymbolPosition, bool hasPath)
        {
            _result = result;
            _pathSymbolPosition = pathSymbolPosition;
            HasPath = hasPath;
        }

        public static int GetPathPosition(string uri, bool hasAuthority)
        {
            if (hasAuthority)
            {
                var uriExcludingAuthorityPrefix = URIComponentsBuilder.GetStringAfterDelimitor(uri, '/').Substring(1);
                if (!uriExcludingAuthorityPrefix.Contains(Delimiters.PATH))
                {
                    return -1;
                }
                return uriExcludingAuthorityPrefix.IndexOf(Delimiters.PATH) +
                       (uri.Length - uriExcludingAuthorityPrefix.Length);
            }

            return uri.IndexOf(Delimiters.PATH);
        }

        public void SetPathWhenIsLastComponent(string uri, int lastIndex)
        {
            _result.Path = uri.Substring(_pathSymbolPosition + 1, lastIndex - _pathSymbolPosition);
        }

        public void SetPathHavingFragmentInformation(string uri, int lastIndex)
        {
            _result.Path = uri.Substring(_pathSymbolPosition + 1, lastIndex - _pathSymbolPosition);
        }

        public void SetPathHavingQueryInformation(string uri, int lastIndex)
        {
            _result.Path = uri.Substring(_pathSymbolPosition + 1, lastIndex - _pathSymbolPosition);
        }
    }
}
