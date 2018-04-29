using System;
using NUnit.Framework;
using URIParser = UriParser.URIParser;

namespace URIParserTests
{
    [TestFixture]
    public class UriParserTests
    {
        public URIParser URIParser;

        [SetUp]
        public void Setup()
        {
            URIParser = new URIParser();
        }

        public class InvalidURI : UriParserTests
        {
            [Test]
            [TestCase("")]
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
            [TestCase("ftp://alex@magic.com")]
            [TestCase("ftp://alex;password1@mas.ro")]
            [TestCase("oneschema://username @com")]
            public void Authentication_username_no_password(string input)
            {
                Assert.That(() => URIParser.Parse(input),
                    Throws.TypeOf<ArgumentException>().With.Message.EqualTo($"\"{input}\" is not a valid URI"));
            }

            [Test]
            [TestCase("ftp://:password99@magic.com:61")]
            [TestCase("ftp://mike:@magic.com:61")]
            [TestCase("ftp://user:password@:61")]
            [TestCase("ftp://user:password@mydomain:/users")]
            [TestCase("ftp://user:password@mydomain:49/?id=343reyery45y4")]
            [TestCase("ftp://user:password@mydomain:49/users?#part3")]
            public void Empty_components_throw_ArgumentException(string input)
            {
                Assert.That(() => URIParser.Parse(input),
                    Throws.TypeOf<ArgumentException>().With.Message.EqualTo($"\"{input}\" is not a valid URI"));
            }

            [Test]
            [TestCase("ftp://user:password@mydomain.com:61?val=31/path")]
            [TestCase("ftp://user:password@mydomain.com:61#frag1/path")]
            [TestCase("ftp://user:password@mydomain.com:61#frag1?x=7")]
            public void Wrong_order_components_throw_ArgumentException(string input)
            {
                Assert.That(() => URIParser.Parse(input),
                    Throws.TypeOf<ArgumentException>().With.Message.EqualTo($"\"{input}\" is not a valid URI"));
            }

            [Test]
            [TestCase("ftp://user:password@mydomain.com:meh/path")]
            public void Not_numerical_port_throws_ArgumentException(string input)
            {
                Assert.That(() => URIParser.Parse(input),
                    Throws.TypeOf<ArgumentException>().With.Message.EqualTo($"\"{input}\" is not a valid URI"));
            }
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

        [Test]
        [TestCase("svn://mih:car@mydomain.com:53")]
        [TestCase("svn://mih:car@mydomain.com:53/docs")]
        [TestCase("svn://mih:car@mydomain.com/files")]
        [TestCase("svn://mih:car@mydomain.com")]
        [TestCase("svn://mih:car@mydomain.com#fig1")]
        [TestCase("svn://mih:car@mydomain.com?param1=3")]
        [TestCase("svn://mih:car@mydomain.com:29/loc1?var=2#fig1")]
        public void Host_is_set_correctly(string input)
        {
            var result = URIParser.Parse(input);
            Assert.That(result.Host, Is.EqualTo("mydomain.com"));
        }

        [Test]
        [TestCase("svn://mih:car@mydomain.com:53")]
        [TestCase("svn://mih:car@ddddd.com:53/docs")]
        [TestCase("svn://mih:car@ppp.com:53/loc1?var=2#fig7")]
        [TestCase("svn://pox.ph:53/loc1?var=2#fig1")]
        public void Port_is_set_correctly(string input)
        {
            var result = URIParser.Parse(input);
            Assert.That(result.Port, Is.EqualTo("53"));
        }

    }

    public class ValidUriPath : UriParserTests
    {
        [Test]
        [TestCase("http://user:pass@website.co.uk:80/authentication")]
        [TestCase("http://user:pass@website.co.uk:80/authentication?admin=1#fig1")]
        [TestCase("http://user:pass@website.co.uk:80/authentication?admin=1")]
        [TestCase("http:/authentication")]
        [TestCase("http:/authentication?admin=1#fig1")]
        [TestCase("http:/authentication?admin=1")]

        public void Path_is_set_correctly(string input)
        {
            var result = URIParser.Parse(input);
            Assert.That(result.Path, Is.EqualTo("authentication"));
        }
    }

    public class ValidUriQuery : UriParserTests
    {
        [Test]
        [TestCase("http://user:pass@website.co.uk:80/authentication?admin=1#fig1")]
        [TestCase("http://user:pass@website.co.uk:80/authentication?admin=1")]
        [TestCase("http:/authentication?admin=1#fig1")]
        [TestCase("http:/authentication?admin=1")]
        [TestCase("http:?admin=1")]
        [TestCase("http:?admin=1#fig3")]

        public void Query_is_set_correctly(string input)
        {
            var result = URIParser.Parse(input);
            Assert.That(result.Query, Is.EqualTo("admin=1"));
        }
    }

    public class ValidUriFragment : UriParserTests
    {
        [Test]
        [TestCase("http://user:pass@website.co.uk:80/authentication?admin=1#fig1")]
        [TestCase("http://user:pass@website.co.uk:80/authentication#fig1")]
        [TestCase("http://user:pass@website.co.uk:80#fig1")]
        [TestCase("http:/authentication?admin=1#fig1")]
        [TestCase("http:/authentication#fig1")]
        [TestCase("http:#fig1")]
        public void Fragment_is_set_correctly(string input)
        {
            var result = URIParser.Parse(input);
            Assert.That(result.Fragment, Is.EqualTo("fig1"));
        }
    }
}
