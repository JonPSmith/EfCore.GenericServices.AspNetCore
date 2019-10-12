// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using TestSupport.EfHelpers;
using Xunit;
using ExampleDatabase;
using ExampleWebApi.BusinessLogic;
using ExampleWebApi.Dtos;
using ExampleWebApi.Helpers;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.ExampleApp
{
    public class TestBusinessLogic
    {

        [Fact]
        public void TestCreateTodoViaBizLogicOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                var service = new CreateTodoBizLogic(context);

                //ATTEMPT
                var dto = new CreateTodoDto
                {
                    Name = "Test",
                    Difficulty = 3
                };
                var result = service.BizAction(dto);
                context.SaveChanges();

                //VERIFY
                service.HasErrors.ShouldBeFalse(service.GetAllErrors());
                context.TodoItems.Single().Name.ShouldEqual("Test");
            }
        }

        [Fact]
        public void TestCreateTodoViaBizLogicWithErrorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                var service = new CreateTodoBizLogic(context);

                //ATTEMPT
                var dto = new CreateTodoDto
                {
                    Name = "Test!",
                    Difficulty = 3
                };
                var result = service.BizAction(dto);

                //VERIFY
                service.HasErrors.ShouldBeTrue(service.GetAllErrors());
                service.GetAllErrors().ShouldEqual("Business logic says the name cannot end with !");
            }
        }


    }
}