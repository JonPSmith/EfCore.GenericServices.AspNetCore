// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using ExampleDatabase;
using ExampleWebApi.BusinessLogic;
using ExampleWebApi.Controllers;
using ExampleWebApi.Dtos;
using ExampleWebApi.Helpers;
using GenericBizRunner;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Test.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.ExampleApp
{
    public class TestToDoController
    {
        [Fact]
        public void TestGetManyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeNameDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var response = controller.Get(service);

                //VERIFY
                response.Value.Count().ShouldEqual(6);
            }
        }

        [Theory]
        [InlineData(2, false)]
        [InlineData(99, true)]
        public void TestGetOneOk(int id, bool shouldBeNull)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeNameDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var response = controller.Get(id, service);

                //VERIFY
                response.CheckResponse(service, shouldBeNull ? null : context.TodoItems.Find(id));
            }
        }

        [Fact]
        public void TestCreateTodoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();
                var controller = new ToDoController();

                var mapper = BizRunnerHelpers.CreateEmptyMapper();
                var bizInstance = new CreateTodoBizLogic(context);
                var service = new ActionService<ICreateTodoBizLogic>(context, bizInstance, mapper);

                //ATTEMPT
                var dto = new CreateTodoDto()
                {
                    Name = "Test",
                    Difficulty = 3,
                };
                var response = controller.Post(dto, service);

                //VERIFY
                response.CheckResponseWithValidCode(service.Status, context.TodoItems.OrderByDescending(x => x.Id).First(), 201);
            }
        }

        [Fact]
        public void TestPutNameOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeNameDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new ChangeNameDto()
                {
                    Id = 2,
                    Name = "Test",
                };
                var response = controller.PutName(dto, service);

                //VERIFY
                response.CheckResponse(service);
            }
        }

        [Fact]
        public void TestPutDifficultyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeDifficultyDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new ChangeDifficultyDto()
                {
                    Id = 1,
                    Difficulty = 5,
                };
                var response = controller.PutDifficuty(dto, service);

                //VERIFY
                response.CheckResponse(service);
            }
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(99, true)]
        public void TestDeleteOk(int id, bool errors)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeDifficultyDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var response = controller.Delete(id, service);

                //VERIFY
                response.CheckResponse(service);
                context.TodoItems.Count().ShouldEqual(errors ? 6 : 5);
            }
        }
    }
}