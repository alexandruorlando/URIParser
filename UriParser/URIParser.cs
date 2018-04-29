using UriParser.Components;
using UriParser.Components.Optional;
using UriParser.Components.Optional.Authority;

namespace UriParser
{
    public class URIParser
    {
        internal string Uri;
        private URIComponentsBuilder _uriComponentsBuilder;
        internal int PathSymbolPosition = -1;
        internal int QuerySymbolPosition = -1;
        internal int FragmentSymbolPosition = -1;
        internal bool HasAuthority;
        internal bool HasPath;
        internal bool HasQuery;
        internal bool HasFragment;
        internal bool HasUser;
        internal bool HasPort;
        internal ParsedURI result;
        private readonly ValidateUriComponents _validateUriComponents;

        public URIParser()
        {
            _validateUriComponents = new ValidateUriComponents(this);
        }
      
        public ParsedURI Parse(string uri)
        {
            Uri = uri;
            result = new ParsedURI();
            _uriComponentsBuilder = new URIComponentsBuilder(uri);
            SetUriComponents(uri);
            return result;
        }

        private void SetUriComponents(string uri)
        {
            IdentifyUriOptionalComponents();
            var schemeInformation = new SchemeInformation(_uriComponentsBuilder);
            schemeInformation.SetScheme(result, uri);
            var authorityInformation = new AuthorityInformation(this);
            if (HasAuthority)
            {
                authorityInformation.SetAuthorityInformation(uri);
            }

            SetOtherOptionalComponents();

            _validateUriComponents.VerifyInformation(result);
        }

        private void SetOtherOptionalComponents()
        {
            var queryInformation = new QueryInformation(result, QuerySymbolPosition, HasQuery);
            var fragmentInformation = new FragmentInformation(result, FragmentSymbolPosition, HasFragment);
            var pathInformation = new PathInformation(result, PathSymbolPosition, HasPath);
            if (HasQuery)
            {
                queryInformation.SetComponentsHavingQuery(Uri, HasPath, PathSymbolPosition);
            }
            if (HasFragment)
            {
                fragmentInformation.SetComponentsHavingFragment(queryInformation, pathInformation, Uri);
            }
            SetOneComponentLeft(queryInformation);
        }

        private void SetOneComponentLeft(QueryInformation queryInformation)
        {
            var lastIndex = Uri.Length - 1;
            if (HasPath && !HasQuery && !HasFragment)
            {
                var pathInformation = new PathInformation(result, PathSymbolPosition, HasPath);
                pathInformation.SetPathWhenIsLastComponent(Uri, lastIndex);
            }

            if (HasQuery && !HasFragment)
            {
                queryInformation.SetQueryWhenIsLastComponent(Uri, lastIndex);
            }
        }

        private void IdentifyUriOptionalComponents()
        {
            HasAuthority = AuthorityInformation.HasAuthorityInformation(Uri);
            PathSymbolPosition = PathInformation.GetPathPosition(Uri, HasAuthority);
            if (PathSymbolPosition > 0)
            {
                HasPath = true;
            }

            QuerySymbolPosition = QueryInformation.GetQueryPosition(Uri);
            if (QuerySymbolPosition > 0)
            {
                HasQuery = true;
            }

            FragmentSymbolPosition = FragmentInformation.GetFragmentPosition(Uri);
            if (FragmentSymbolPosition > 0)
            {
                HasFragment = true;
            }

            _validateUriComponents.CheckOptionalComponentsValidity(Uri);
        }
    }
}
