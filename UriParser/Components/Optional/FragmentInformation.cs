namespace UriParser.Components.Optional
{
    public class FragmentInformation
    {
        private readonly ParsedURI _result;
        private readonly int _fragmentSymbolPosition;
        public bool HasFragment;

        public FragmentInformation(ParsedURI result, int fragmentSymbolPosition, bool hasFragment)
        {
            _result = result;
            _fragmentSymbolPosition = fragmentSymbolPosition;
            HasFragment = hasFragment;
        }

        public void SetComponentsHavingFragment(QueryInformation queryInformation, PathInformation pathInformation, string uri)
        {
            var lastIndex = _fragmentSymbolPosition - 1;
            if (pathInformation.HasPath && !queryInformation.HasQuery)
            {
                pathInformation.SetPathHavingFragmentInformation(uri, lastIndex);
            }

            if (queryInformation.HasQuery)
            {
                queryInformation.SetQueryHavingFragmentInformation(uri, lastIndex);
            }

            lastIndex = uri.Length - 1;
            SetFragment(uri, lastIndex);
        }
        public static int GetFragmentPosition(string uri)
        {
            return uri.IndexOf(Delimiters.FRAGMENT);
        }
        private void SetFragment(string uri, int lastIndex)
        {
            _result.Fragment = uri.Substring(_fragmentSymbolPosition + 1, lastIndex - _fragmentSymbolPosition);
        }
    }
}
