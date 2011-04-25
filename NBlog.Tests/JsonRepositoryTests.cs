using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlog.Web.Application;
using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Application.Storage;
using NBlog.Web.Application.Storage.Json;

namespace NBlog.Tests
{
    [TestClass]
    public class JsonRepositoryTests
    {
        public TestContext TestContext { get; set; }
        private RepositoryKeys _keys;
        private JsonRepository _jsonRepository;
        private string _dataPath;

        [TestInitialize]
        public void TestInit()
        {
            _keys = new RepositoryKeys();
            _keys.Add<Entry>(e => e.Slug);
            _keys.Add<Config>(c => c.Site);
            _keys.Add<User>(u => u.Username);

            _dataPath = Path.Combine(TestContext.TestDir, "JsonRepository");
            _jsonRepository = new JsonRepository(_keys, _dataPath);
        }


        [TestCleanup]
        public void TestCleanup()
        {
            if (Directory.Exists(_dataPath))
            {
                Directory.Delete(_dataPath, recursive: true);
            }
        }


        [TestMethod]
        public void Single_Should_Return_Correct_Entity_By_Key()
        {
            // arrange
            const string title = "Top 10 C# Tips";
            var slug = title.ToUrlSlug();
            var entry = new Entry { Slug = slug, Title = title };

            // act
            _jsonRepository.Save(entry);
            
            var keyValue = _keys.GetKeyValue(entry);
            var retrievedEntry = _jsonRepository.Single<Entry>(keyValue);

            // assert
            Assert.AreEqual(retrievedEntry.Title, title);
        }


        [TestMethod]
        public void List_Should_Return_All_Entities()
        {
            // arrange
            _jsonRepository.Save(new Entry { Slug = "entry-1", Title = "Entry 1" });
            _jsonRepository.Save(new Entry { Slug = "entry-2", Title = "Entry 2" });
            _jsonRepository.Save(new Entry { Slug = "entry-3", Title = "Entry 3" });

            // act
            var all = _jsonRepository.All<Entry>();

            // assert
            Assert.IsTrue(all.Count() == 3);
        }

        [TestMethod]
        public void Exists_Should_Be_True_When_Entity_Exists()
        {
            // arrange
            _jsonRepository.Save(new Entry { Slug = "entry-1", Title = "Entry 1" });

            // act
            var exists = _jsonRepository.Exists<Entry>("entry-1");

            // assert
            Assert.IsTrue(exists);
        }


        [TestMethod]
        public void Exists_Should_Be_False_When_Entity_Deleted()
        {
            // arrange
            _jsonRepository.Save(new Entry { Slug = "entry-1", Title = "Entry 1" });
            _jsonRepository.Delete<Entry>("entry-1");

            // act
            var exists = _jsonRepository.Exists<Entry>("entry-1");

            // assert
            Assert.IsFalse(exists);
        }
    }
}
