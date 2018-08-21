// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using ExampleDatabase;
using ExampleWebApi.BusinessLogic;
using ExampleWebApi.Controllers;
using ExampleWebApi.Dtos;
using ExampleWebApi.Helpers;
using GenericBizRunner;
using GenericServices.AspNetCore;
using GenericServices.AspNetCore.UnitTesting;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Test.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.ExampleApp
{
    public class IntegrationTestToDoController
    {
        private readonly IGenericServicesConfig _genericServiceConfig = new GenericServicesConfig
        {
            DtoAccessValidateOnSave = true, //This causes validation to happen on create/update via DTOs
            DirectAccessValidateOnSave = true, //This causes validation to happen on direct create/update and delete
            NoErrorOnReadSingleNull = true //When working with WebAPI you should set this flag. Responce then sends 404 on null result
        };

        [Fact]
        public async Task TestGetManyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeNameDto>(_genericServiceConfig);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var response = await controller.GetAsync(service);

                //VERIFY
                response.GetStatusCode().ShouldEqual(CreateResponse.OkStatusCode);
                var rStatus = response.CopyToStatus();
                rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
                rStatus.Result.Count.ShouldEqual(6);
            }
        }

        [Fact]
        public void TestGetOneOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeNameDto>(_genericServiceConfig);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var response = controller.Get(1, service);

                //VERIFY
                response.GetStatusCode().ShouldEqual(CreateResponse.OkStatusCode);
                var rStatus = response.CopyToStatus();
                rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
                rStatus.Message.ShouldEqual("Success");
            }
        }

        [Fact]
        public void TestGetOneNullReturn()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeNameDto>(_genericServiceConfig);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var response = controller.Get(99, service);

                //VERIFY
                response.GetStatusCode().ShouldEqual(CreateResponse.ResultIsNullStatusCode);
                var rStatus = response.CopyToStatus();
                rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
                rStatus.Message.ShouldEqual("The Todo Item was not found.");
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
                response.GetStatusCode().ShouldEqual(201);
                var rStatus = response.CopyToStatus();
                rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
                rStatus.Message.ShouldEqual("Successfully saved the todo item 'Test'.");
                rStatus.Result.Id.ShouldNotEqual(0);
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
                var utData = context.SetupSingleDtoAndEntities<ChangeNameDto>(_genericServiceConfig);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new ChangeNameDto()
                {
                    Id = 2,
                    Name = "Test",
                };
                var response = controller.PutName(dto, service);

                //VERIFY
                response.GetStatusCode().ShouldEqual(CreateResponse.OkStatusCode);
                var rStatus = response.CopyToStatus();
                rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
                rStatus.Message.ShouldEqual("Successfully updated the Todo Item");
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
                var utData = context.SetupSingleDtoAndEntities<ChangeDifficultyDto>(_genericServiceConfig);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new ChangeDifficultyDto()
                {
                    Id = 1,
                    Difficulty = 5,
                };
                var response = controller.PutDifficuty(dto, service);

                //VERIFY
                response.GetStatusCode().ShouldEqual(CreateResponse.OkStatusCode);
                var rStatus = response.CopyToStatus();
                rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
                rStatus.Message.ShouldEqual("Successfully updated the Todo Item");
            }
        }

        [Fact]
        public void TestPutDifficultyValidationError()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeDifficultyDto>(_genericServiceConfig);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new ChangeDifficultyDto()
                {
                    Id = 1,
                    Difficulty = 99,
                };
                var response = controller.PutDifficuty(dto, service);

                //VERIFY
                response.GetStatusCode().ShouldEqual(CreateResponse.ResultIsNullStatusCode);
                var rStatus = response.CopyToStatus();
                rStatus.IsValid.ShouldBeFalse();
                rStatus.GetAllErrors().ShouldEqual("The field Difficulty must be between 1 and 5.");
            }
        }

        [Fact]
        public void TestDeleteOK()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeDifficultyDto>(_genericServiceConfig);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var response = controller.Delete(2, service);

                //VERIFY
                response.GetStatusCode().ShouldEqual(CreateResponse.OkStatusCode);
                var rStatus = response.CopyToStatus();
                rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
                rStatus.Message.ShouldEqual("Successfully deleted a Todo Item");
                context.TodoItems.Count().ShouldEqual(5);
            }
        }

        [Fact]
        public void TestDeleteNotFound()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeDifficultyDto>(_genericServiceConfig);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var response = controller.Delete(99, service);

                //VERIFY
                response.GetStatusCode().ShouldEqual(CreateResponse.ResultIsNullStatusCode);
                var rStatus = response.CopyToStatus();
                rStatus.IsValid.ShouldBeFalse();
                rStatus.GetAllErrors().ShouldEqual("Sorry, I could not find the Todo Item you wanted to delete.");
            }
        }

    }
}