using System.Collections.Generic;
using MbUnit.Framework;
using NBlog.Web.Application.Infrastructure;

namespace NBlog.Tests
{
    [TestFixture]
    public class SlugTests
    {
        [Test]
        public void ToSlugUrl_Should_Build_Correct_Slugs()
        {
            // arrange
            var slugs = new Dictionary<string, string>
            {
                {"myentry", "myentry"},
                {"my-entry", "my-entry"},
                {"MyEntry", "myentry"},
                {"my entry", "my-entry"},
                {" my  entry ", "my-entry"},
                {"my<script></script>entry", "my-script-script-entry"},
                {">my.'& entry*?", "my-entry"}
            };

            // act
            foreach (var slug in slugs)
            {
                // assert
                Assert.AreEqual(slug.Key.ToUrlSlug(), slug.Value);
            }
        }
    }
}
