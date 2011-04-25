using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;
using NBlog.Web.Application.Service;
using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Controllers;
using NSubstitute;

namespace NBlog.Tests
{
    [TestFixture]
    public class EntryControllerTests
    {
        [Test]
        public void ListTest()
        {
            // arrange
            var services = Substitute.For<IServices>();
            services.Entry.GetList().Returns(new List<Entry>(new[] {
                new Entry{ Title = "Entry 1" },
                new Entry{ Title = "Entry 2" },  
                new Entry{ Title = "Entry 3" },  
            }));

            // act
            var controller = new HomeController(services);
            var model = controller.Index().ViewData.Model as HomeController.IndexModel;

            // assert
            services.Entry.Received().GetList();
            Assert.AreEqual(model.Entries.Count(), services.Entry.GetList().Count);
        }
    }
}
