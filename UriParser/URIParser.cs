
using System.Runtime.InteropServices;

namespace UriParser
{
    public class URIParser
    {
        private string _uri;
        private URIComponentsHelper _uriComponentsHelper;
        private const string _authorityPrefix = "//";

        public ParsedURI Parse(string uri)
        {
            var result = new ParsedURI();
            _uriComponentsHelper = new URIComponentsHelper(uri);
            var scheme = _uriComponentsHelper.GetScheme();
            _uriComponentsHelper.CheckPartOfURI(URIComponent.Scheme, scheme);
            result.Scheme = scheme;
            var restOfURI = _uriComponentsHelper.GetStringAfterDelimitor(uri, Delimiters.SCHEME);
            if (IsAuthorityPart(restOfURI))
            {
                var uriAfterSlashes = restOfURI.Substring(2);
                result.User = _uriComponentsHelper.GetUser(uriAfterSlashes);
                restOfURI = _uriComponentsHelper.GetStringAfterDelimitor(uriAfterSlashes, Delimiters.USER);
                result.Password = _uriComponentsHelper.GetPassowrd(restOfURI);
            }
            else
            {

            }

            return result;
        }

        private bool IsAuthorityPart(string restOfUri)
        {
            if (restOfUri.Length <= 2)
            {
                return false;
            }
            
            var twoSlashesPart = restOfUri.Substring(0, 2);
            
            return twoSlashesPart.Equals(_authorityPrefix);
        }
    }
}
