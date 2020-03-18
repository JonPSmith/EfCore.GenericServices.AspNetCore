// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using CommonWebParts;
using TestSupport.EfHelpers;
using Xunit;
using ExampleDatabase;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.ExampleApp
{
    public class TestExampleDbContext
    {
        [Fact]
        public void TestSeedDatabaseOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.SeedDatabase();

                //VERIFY
                context.TodoItems.Count().ShouldEqual(6);
            }
        }

        [Fact]
        public void TestChangeNameOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new TodoItem("Test", 1));
                context.SaveChanges();

                //ATTEMPT
                var item = context.TodoItems.Single();
                var status = item.ChangeName("New Name");
                context.SaveChanges();

                //VERIFY
                status.IsValid.ShouldBeTrue(status.GetAllErrors());
                context.TodoItems.Single().Name.ShouldEqual("New Name");
            }
        }

        [Fact]
        public void TestChangeNameWithErrorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new TodoItem("Test", 1));
                context.SaveChanges();

                //ATTEMPT
                var item = context.TodoItems.Single();
                var status = item.ChangeName("New Name!");

                //VERIFY
                status.IsValid.ShouldBeFalse(status.GetAllErrors());
                status.GetAllErrors().ShouldEqual("Business logic says the name canot end with !");
            }
        }

        [Fact]
        public void TestChangeDifficultyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new TodoItem("Test", 1));
                context.SaveChanges();

                //ATTEMPT
                var item = context.TodoItems.Single();
                item.ChangeDifficulty(2);
                context.SaveChanges();

                //VERIFY
                context.TodoItems.Single().Difficulty.ShouldEqual(2);
            }
        }
    }
}