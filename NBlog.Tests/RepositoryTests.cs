using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using MbUnit.Framework;
using NBlog.Web.Application;
using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Application.Storage;
using NBlog.Web.Application.Storage.Json;
using NBlog.Web.Application.Storage.Sql;

namespace NBlog.Tests
{
    public class RepositoryTests
    {
        private static readonly RepositoryKeys Keys;
        private static readonly string JsonWorkingFolder;
        private static readonly string SqlConnectionString;
        private static readonly string SqlDatabaseName;

        static RepositoryTests()
        {
            Keys = new RepositoryKeys();
            Keys.Add<Entry>(e => e.Slug);
            Keys.Add<Config>(c => c.Site);
            Keys.Add<User>(u => u.Username);

            JsonWorkingFolder = Path.Combine(Path.GetTempPath(), "NBlogIntegrationTests");
            SqlConnectionString = "Server=.;Trusted_Connection=True;";
            SqlDatabaseName = "NBlogIntegrationTests";
        }


        [TearDown]
        public void TestCleanup()
        {
            var repositoryType = Instance.GetType();

            if (repositoryType == typeof(JsonRepository))
            {
                if (Directory.Exists(JsonWorkingFolder))
                {
                    Directory.Delete(JsonWorkingFolder, recursive: true);
                }
            }
            else if (repositoryType == typeof(SqlRepository))
            {
                using (var cnn = new SqlConnection(SqlConnectionString))
                using (var cmd = new SqlCommand("EXEC sp_MSforeachtable @command1 = 'TRUNCATE TABLE ?'", cnn))
                {
                    cnn.Open();
                    cnn.ChangeDatabase(SqlDatabaseName);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        [FixtureTearDown]
        public void FixtureTearDown()
        {
            if (Instance.GetType() == typeof(SqlRepository))
            {
                const string dropSql = @"
                    ALTER DATABASE [{0}] SET OFFLINE WITH ROLLBACK IMMEDIATE
                    DROP DATABASE [{0}]";

                using (var cnn = new SqlConnection(SqlConnectionString))
                using (var cmd = new SqlCommand(string.Format(dropSql, SqlDatabaseName), cnn))
                {
                    cnn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static IEnumerable<IRepository> GetInstances()
        {
            yield return BuildJsonRepository();
            yield return BuildSqlRepository();
        }


        [Factory("GetInstances")]
        public IRepository Instance;


        private static JsonRepository BuildJsonRepository()
        {
            return new JsonRepository(Keys, JsonWorkingFolder);
        }


        private static SqlRepository BuildSqlRepository()
        {
            return new SqlRepository(Keys, SqlConnectionString, SqlDatabaseName);
        }


        [Test]
        public void Single_Should_Return_Correct_Entity_By_Key()
        {
            // arrange
            var repository = Instance;
            const string title = "Top 10 C# Tips";
            var slug = title.ToUrlSlug();
            var entry = new Entry { Slug = slug, Title = title, DateCreated = DateTime.Now };

            // act
            repository.Save(entry);

            var keyValue = Keys.GetKeyValue(entry);
            var retrievedEntry = repository.Single<Entry>(keyValue);

            // assert
            Assert.AreEqual(retrievedEntry.Title, title);
        }


        [Test]
        public void List_Should_Return_All_Entities()
        {
            // arrange
            var repository = Instance;
            repository.Save(new Entry { Slug = "entry-1", Title = "Entry 1", DateCreated = DateTime.Now });
            repository.Save(new Entry { Slug = "entry-2", Title = "Entry 2", DateCreated = DateTime.Now });
            repository.Save(new Entry { Slug = "entry-3", Title = "Entry 3", DateCreated = DateTime.Now });

            // act
            var all = repository.All<Entry>();

            // assert
            Assert.IsTrue(all.Count() == 3);
        }


        [Test]
        public void Exists_Should_Be_True_When_Entity_Exists()
        {
            // arrange
            var repository = Instance;
            repository.Save(new Entry { Slug = "entry-1", Title = "Entry 1", DateCreated = DateTime.Now });

            // act
            var exists = repository.Exists<Entry>("entry-1");

            // assert
            Assert.IsTrue(exists);
        }


        [Test]
        public void Exists_Should_Be_False_When_Entity_Deleted()
        {
            // arrange
            var repository = Instance;
            repository.Save(new Entry { Slug = "entry-1", Title = "Entry 1", DateCreated = DateTime.Now });
            repository.Delete<Entry>("entry-1");

            // act
            var exists = repository.Exists<Entry>("entry-1");

            // assert
            Assert.IsFalse(exists);
        }
    }
}