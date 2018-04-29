namespace UriParser.Components.Optional.Authority
{
    public class UserInformation
    {
        private readonly URIComponentsBuilder _uriComponentsBuilder;
        private readonly ParsedURI _result;
        private string _authorityInformation;

        public UserInformation(URIComponentsBuilder uriComponentsBuilder, ParsedURI result, string authorityInformation)
        {
            _uriComponentsBuilder = uriComponentsBuilder;
            _result = result;
            _authorityInformation = authorityInformation;
        }

        public void SetPassword()
        {
            _result.Password = _uriComponentsBuilder.GetPassword(_authorityInformation);
        }

        public void SetUser()
        {
            _result.User = _uriComponentsBuilder.GetUser(_authorityInformation);
            _authorityInformation =
                URIComponentsBuilder.GetStringAfterDelimitor(_authorityInformation, Delimiters.USER);
        }
    }
}
