using System;
using NUnit.Framework;
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
            public void Not_valid_schema_throws_ArgumentException(string input)
            {
                Assert.That(() => URIParser.Parse(input), Throws.TypeOf<ArgumentException>().With.Message.EqualTo($"\"{input}\" is not a valid URI"));
            }
        } 
    }
}
