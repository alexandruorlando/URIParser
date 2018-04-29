namespace UriParser.Components.Optional
{
    public class QueryInformation
    {
        private readonly ParsedURI _result;
        private readonly int _querySymbolPosition;
        public bool HasQuery;
        public QueryInformation(ParsedURI result, int querySymbolPosition, bool hasQuery)
        {
            _result = result;
            _querySymbolPosition = querySymbolPosition;
            HasQuery = hasQuery;
        }

        public void SetComponentsHavingQuery(string uri, bool hasPath, int pathSymbolPosition)
        {
            var lastIndex = _querySymbolPosition - 1;
            if (hasPath)
            {
                var pathInformation = new PathInformation(_result, pathSymbolPosition, hasPath);
                pathInformation.SetPathHavingQueryInformation(uri, lastIndex);
            }
        }

        public void SetQueryWhenIsLastComponent(string uri, int lastIndex)
        {
            _result.Query = uri.Substring(_querySymbolPosition + 1, lastIndex - _querySymbolPosition);
        }

        public void SetQueryHavingFragmentInformation(string uri, int lastIndex)
        {
            _result.Query = uri.Substring(_querySymbolPosition + 1, lastIndex - _querySymbolPosition);
        }

        public static int GetQueryPosition(string uri)
        {
            return uri.IndexOf(Delimiters.QUERY);
        }
    }
}
