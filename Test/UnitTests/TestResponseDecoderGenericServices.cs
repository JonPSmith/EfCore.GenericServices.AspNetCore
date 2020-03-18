// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using AspNetCore.UnitTesting;
using CommonWebParts.Dtos;
using ExampleWebApi.Controllers;
using GenericServices.AspNetCore;
using StatusGeneric;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests
{
    public class TestResponseDecoderGenericServices
    {
        [Fact]
        public void TestStatusIsValidOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var actionResult = status.Response();

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CopyToStatus();

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.OkStatusCode);
            rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
            rStatus.Message.ShouldEqual("Success");
        }

        [Fact]
        public void TestStatusIsValidWithStatusCodeOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var actionResult = status.ResponseWithValidCode(201);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();

            //VERIFY
            statusCode.ShouldEqual(201);
        }


        [Fact]
        public void TestStatusHasErrorOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            status.AddError("An Error", "MyProp");
            var actionResult = status.Response();

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CopyToStatus();

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.ErrorsStatusCode);
            rStatus.IsValid.ShouldBeFalse();
            rStatus.Errors.Count.ShouldEqual(1);
            rStatus.Errors.Single().ErrorResult.ErrorMessage.ShouldEqual("An Error");
            rStatus.Errors.Single().ErrorResult.MemberNames.ShouldEqual(new []{"MyProp"});
        }

        [Fact]
        public void TestStatusHasTwoErrorOnSamePropOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            status.AddError("An Error", "MyProp");
            status.AddError("Another Error", "MyProp");
            var actionResult = status.Response();

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CopyToStatus();

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.ErrorsStatusCode);
            rStatus.IsValid.ShouldBeFalse();
            rStatus.Errors.Count.ShouldEqual(2);
            rStatus.Errors[0].ErrorResult.ErrorMessage.ShouldEqual("An Error");
            rStatus.Errors[0].ErrorResult.MemberNames.ShouldEqual(new[] { "MyProp" });
            rStatus.Errors[1].ErrorResult.ErrorMessage.ShouldEqual("Another Error");
            rStatus.Errors[1].ErrorResult.MemberNames.ShouldEqual(new[] { "MyProp" });
        }

        //------------------------------------------------------
        //with return result

        [Fact]
        public void TestStatusIsValidWithResultOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var actionResult = status.Response(1);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CopyToStatus();

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.OkStatusCode);
            rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
            rStatus.Message.ShouldEqual("Success");
            rStatus.Result.ShouldEqual(1);
        }

        [Fact]
        public void TestStatusIsValidWithResultAndStatusCodeOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var actionResult = status.ResponseWithValidCode(1, 201);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CopyToStatus();

            //VERIFY
            statusCode.ShouldEqual(201);
            rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
            rStatus.Result.ShouldEqual(1);
        }

        [Fact]
        public void TestStatusIsValidWithClassResultOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var actionResult = status.Response("Hello");

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CopyToStatus();

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.OkStatusCode);
            rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
            rStatus.Result.ShouldEqual("Hello");
        }

        [Fact]
        public void TestStatusIsValidWithClassResultNullOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var actionResult = status.Response<string>(null);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CopyToStatus();

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.ResultIsNullStatusCode);
            rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
            rStatus.Result.ShouldEqual(null);
        }

        [Fact]
        public void TestStatusIsValidWithClassResultNullAndStatusCodeOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var actionResult = status.ResponseWithValidCode<string>(null, CreateResponse.OkStatusCode, 204);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CopyToStatus();

            //VERIFY
            statusCode.ShouldEqual(204);
            rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
            rStatus.Result.ShouldEqual(null);
        }

        [Fact]
        public void TestStatusErrorWithResultOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            status.AddError("Bad");
            var actionResult = status.Response(1);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CopyToStatus();

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.ErrorsStatusCode);
            rStatus.IsValid.ShouldBeFalse();
            rStatus.Errors.Count.ShouldEqual(1);
            rStatus.Errors.Single().ErrorResult.ErrorMessage.ShouldEqual("Bad");
            rStatus.Errors.Single().ErrorResult.MemberNames.ShouldEqual(new[] { "" });
        }

        [Fact]
        public void TestStatusErrorWithClassResultNullOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            status.AddError("Bad");
            var actionResult = status.Response<string>(null);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CopyToStatus();

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.ErrorsStatusCode);
            rStatus.IsValid.ShouldBeFalse();
            rStatus.Errors.Count.ShouldEqual(1);
            rStatus.Errors.Single().ErrorResult.ErrorMessage.ShouldEqual("Bad");
            rStatus.Errors.Single().ErrorResult.MemberNames.ShouldEqual(new[] { "" });
        }

        //----------------------------------------------------
        //Test Create response

        [Fact]
        public void TestCreateStatusOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var controller = new ToDoController();
            var dto = new CreateTodoDto();
            var actionResult = status.Response(controller, "Get", new { id = 7 }, dto);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CheckCreateResponse("Get", new { id = 7 }, dto);

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.CreatedStatusCode);
            rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
            rStatus.Message.ShouldEqual("Success");
        }

        [Fact]
        public void TestCreateStatusWithErrorsOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var controller = new ToDoController();
            var dto = new CreateTodoDto();

            status.AddError("An error");
            var actionResult = status.Response(controller, "Get", new { id = 7 }, dto);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CheckCreateResponse("Get", new { id = 7 }, dto);

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.ErrorsStatusCode);
            rStatus.IsValid.ShouldBeTrue(rStatus.GetAllErrors());
            rStatus.Message.ShouldEqual("Errors: An error");
        }

        [Fact]
        public void TestCreateStatusBadRouteNameOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var controller = new ToDoController();
            var dto = new CreateTodoDto();
            var actionResult = status.Response(controller, "Bad", new { id = 7 }, dto);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CheckCreateResponse("Get", new { id = 7 }, dto);

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.CreatedStatusCode);
            rStatus.IsValid.ShouldBeFalse(rStatus.GetAllErrors());
            rStatus.GetAllErrors().ShouldEqual("RouteName: expected Get, found: Bad");
        }

        [Fact]
        public void TestCreateStatusBadDtoOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var controller = new ToDoController();
            var dto = new CreateTodoDto();
            var actionResult = status.Response(controller, "Get", new { id = 7 }, dto);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CheckCreateResponse("Get", new { id = 7 }, new CreateTodoDto());

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.CreatedStatusCode);
            rStatus.IsValid.ShouldBeFalse(rStatus.GetAllErrors());
            rStatus.GetAllErrors().ShouldEqual("DTO: the returned DTO instance does not match the test DTO: expected CreateTodoDto, found: CreateTodoDto");
        }

        [Fact]
        public void TestCreateStatusRouteValueNamesNotMatchOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var controller = new ToDoController();
            var dto = new CreateTodoDto();
            var actionResult = status.Response(controller, "Get", new { bad = 7 }, dto);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CheckCreateResponse("Get", new { id = 7 }, dto);

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.CreatedStatusCode);
            rStatus.IsValid.ShouldBeFalse(rStatus.GetAllErrors());
            rStatus.GetAllErrors().ShouldEqual("RouteValues: Different named properties: expected = id, found = bad");
        }

        [Fact]
        public void TestCreateStatusRouteValueTypeNotMatchOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var controller = new ToDoController();
            var dto = new CreateTodoDto();
            var actionResult = status.Response(controller, "Get", new { id = "hello" }, dto);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CheckCreateResponse("Get", new { id = 7 }, dto);

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.CreatedStatusCode);
            rStatus.IsValid.ShouldBeFalse(rStatus.GetAllErrors());
            rStatus.GetAllErrors().ShouldEqual("RouteValues->id, different type: expected = Int32, found = String");
        }

        [Fact]
        public void TestCreateStatusRouteValueValueNotMatchOk()
        {
            //SETUP
            var status = new StatusGenericHandler();
            var controller = new ToDoController();
            var dto = new CreateTodoDto();
            var actionResult = status.Response(controller, "Get", new { id = 999 }, dto);

            //ATTEMPT
            var statusCode = actionResult.GetStatusCode();
            var rStatus = actionResult.CheckCreateResponse("Get", new { id = 7 }, dto);

            //VERIFY
            statusCode.ShouldEqual(CreateResponse.CreatedStatusCode);
            rStatus.IsValid.ShouldBeFalse(rStatus.GetAllErrors());
            rStatus.GetAllErrors().ShouldEqual("RouteValues->id, different values: expected = 7, found = 999");
        }
    }
}