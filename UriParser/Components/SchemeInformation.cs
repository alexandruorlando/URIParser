namespace UriParser.Components
{
    public class SchemeInformation
    {
        private readonly URIComponentsBuilder _uriComponentsBuilder;

        public SchemeInformation(URIComponentsBuilder uriComponentsBuilder)
        {
            _uriComponentsBuilder = uriComponentsBuilder;
        }
        public void SetScheme(ParsedURI result, string uri)
        {
            result.Scheme = _uriComponentsBuilder.GetScheme(uri);
        }
    }
}
