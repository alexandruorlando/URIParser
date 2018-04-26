
namespace UriParser
{
    public class URIParser
    {
        private string _uri;
        private URIComponentsHelper _uriComponentsHelper;

        public void Parse(string uri)
        {
            _uriComponentsHelper = new URIComponentsHelper(uri);
            var scheme = _uriComponentsHelper.GetScheme();
            var restOfURI = _uriComponentsHelper.GetStringAfterDelimitor(uri, Delimiters.SCHEME);
            _uriComponentsHelper.CheckPartOfURI(URIComponent.Scheme, scheme);
        }
    }
}
