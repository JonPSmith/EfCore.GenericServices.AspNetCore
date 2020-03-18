// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using AspNetCore.UnitTesting;
using CommonWebParts;
using CommonWebParts.Dtos;
using ExampleDatabase;
using ExampleWebApi.Controllers;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using GenericServices.AspNetCore;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
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
            NoErrorOnReadSingleNull = true //When working with WebAPI you should set this flag. Response then sends 204 on null result
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
                var response = await controller.GetManyAsync(service);

                //VERIFY
                response.GetStatusCode().ShouldEqual(CreateResponse.OkStatusCode);
                var rStatus = response.CopyToStatus();
                rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
                rStatus.Result.Count.ShouldEqual(6);
            }
        }

        [Fact]
        public async Task TestGetOneOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeNameDto>(_genericServiceConfig);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var response = await controller.GetSingleAsync(1, service);

                //VERIFY
                response.GetStatusCode().ShouldEqual(CreateResponse.OkStatusCode);
                var rStatus = response.CopyToStatus();
                rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
                rStatus.Message.ShouldEqual("Success");
            }
        }

        [Fact]
        public async Task TestGetOneNullReturn()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ExampleDbContext>();
            using (var context = new ExampleDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabase();

                var controller = new ToDoController();
                var utData = context.SetupSingleDtoAndEntities<ChangeNameDto>(_genericServiceConfig);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var response = await controller.GetSingleAsync(99, service);

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

                var noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };
                var utData = new NonDiBizSetup(noCachingConfig);
                var bizInstance = new CreateTodoBizLogic(context);
                var service = new ActionService<ICreateTodoBizLogic>(context, bizInstance, utData.WrappedConfig);

                //ATTEMPT
                var dto = new CreateTodoDto()
                {
                    Name = "Test",
                    Difficulty = 3,
                };
                var response = controller.Post(dto, service);

                //VERIFY
                response.GetStatusCode().ShouldEqual(201);
                var rStatus = response.CheckCreateResponse("GetSingleTodo", new {id = 7}, context.TodoItems.Find(7));
                rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
                rStatus.Message.ShouldEqual("Success");
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
                var response = controller.Name(dto, service);

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
                var response = controller.Difficulty(dto, service);

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
                var response = controller.Difficulty(dto, service);

                //VERIFY
                response.GetStatusCode().ShouldEqual(CreateResponse.ErrorsStatusCode);
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
                response.GetStatusCode().ShouldEqual(CreateResponse.ErrorsStatusCode);
                var rStatus = response.CopyToStatus();
                rStatus.IsValid.ShouldBeFalse();
                rStatus.GetAllErrors().ShouldEqual("Sorry, I could not find the Todo Item you wanted to delete.");
            }
        }

    }
}