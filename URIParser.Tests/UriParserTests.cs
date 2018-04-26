using System;
using System.Security.Authentication;
using NUnit.Framework;
using UriParser;
using UriParser.Exceptions;
using URIParser = UriParser.URIParser;

namespace URIParserTests
{
    [TestFixture]
    public class UriParserTests
    {
        public URIParser URIParser;

        [OneTimeSetUp]
        public void Setup()
        {
            URIParser = new URIParser();
        }

        public class InvalidURI : UriParserTests
        {
            [Test]
            [TestCase("1\\http:")]
            [TestCase("")]
            [TestCase("::")]
            [TestCase("a@bc:")]
            public void Not_valid_scheme_throws_ArgumentException(string input)
            {
                Assert.That(() => URIParser.Parse(input),
                    Throws.TypeOf<ArgumentException>().With.Message.EqualTo($"\"{input}\" is not a valid URI"));
            }

            [Test]
            [TestCase("ftp://alex")]
            [TestCase("ftp://alex;password1")]
            [TestCase("oneschema://username ")]
            public void Authentication_username_no_password(string input)
            {
                Assert.That(() => URIParser.Parse(input),
                    Throws.TypeOf<ArgumentException>().With.Message.EqualTo($"\"{input}\" is not a valid URI"));
            }
        }

        public class ValidUriScheme : UriParserTests
        {
            [Test]
            [TestCase("http:")]
            [TestCase("a1111:")]
            [TestCase("x:")]
            public void Only_scheme_is_set(string input)
            {
                var result = URIParser.Parse(input);
                var schemeFromInput = input.Substring(0, input.Length - 1);
                Assert.That(result.Scheme, Is.EqualTo(schemeFromInput));
            }
        }

        public class ValidUriAuthority : UriParserTests
        {
            [Test]
            [TestCase("https://alex:pass123@home-address")]
            public void Scheme_is_set_correctly(string input)
            {
                var result = URIParser.Parse(input);
                Assert.That(result.Scheme, Is.EqualTo("https"));
            }

            [Test]
            [TestCase("https://alex:pass123@home-address")]
            public void User_is_set_correctly(string input)
            {
                var result = URIParser.Parse(input);
                Assert.That(result.User, Is.EqualTo("alex"));
            }

            [Test]
            [TestCase("https://alex:pass123@home-address")]
            public void Password_is_set_correctly(string input)
            {
                var result = URIParser.Parse(input);
                Assert.That(result.Password, Is.EqualTo("pass123"));
            }
        }
    }
}
