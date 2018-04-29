# URIParser

The URIParser was implemented in the URIParser project. The main function can be found in URIParser.cs. URIParser.Parse function receives an URI as a parameter and it returns a ParsedURI objects having set all the components provided in the URI parameter.
If one of the component was not provided in the URI, in the ParsedURI object, that component has value NULL.
In case an invalid URI is received as input, an ArgumentException is thrown.

The parser was implemented with TDD.
The tests can be found in the URIParser.Tests project.
