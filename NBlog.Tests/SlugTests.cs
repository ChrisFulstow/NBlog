using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlog.Web.Application;

namespace NBlog.Tests
{
    [TestClass]
    public class SlugTests
    {
        [TestMethod]
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