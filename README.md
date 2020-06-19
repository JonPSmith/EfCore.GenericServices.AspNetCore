# EfCore.GenericServices.AspNetCore

This library provides converters from [EfCore.GenericService](https://github.com/JonPSmith/EfCore.GenericServices) and [EfCore.GenericBizRunner](https://github.com/JonPSmith/EfCore.GenericBizRunner) status results to into two ASP.NET Core formats. 

1. [ASP.NET Core MVC or Razor pages](#1-aspnet-core-mvc-or-razor-pages---copy-status-to-modelstate) - copy to `ModelState`.
2. [ASP.NET Core Web API](#2-aspnet-core-web-api---forming-the-correct-http-response) - form correct HTTP response.

The library also contains code for unit/integration testing of code that uses EfCore.GenericService or EfCore.GenericBizRunner in ASP.NET Core controllers. See information at [end of the Readme file](#unit-testintegration-test-of-web-apis).

*NOTE: If you are interested in using EfCore.GenericService or EfCore.GenericBizRunner in Web APIs then see the article
["How to write good, testable ASP.NET Core Web API code quickly"](https://www.thereformedprogrammer.net/how-to-write-good-testable-asp-net-core-web-api-code-quickly/) for more information.

MIT licence - available on [NuGet](https://www.nuget.org/packages/EfCore.GenericServices.AspNetCore/).

## 1. ASP.NET Core MVC or Razor pages - copy status to `ModelState`

The extension method `CopyErrorsToModelState` which copy `IStatusGeneric` errors into ASP.NET Core's `ModelState`. Useful when working html/razor pages.

There are two forms of the method `CopyErrorsToModelState` - they are:

### 1a. `CopyErrorsToModelState` taking a dto

In ASP.NET Core MVC or Razor pages you need to return errors vai the ModelState, which has the property name and the error for that property. However it is important to NOT have a named property that doesn't appear in the class shown on the display, otherwise the error doesn't appear.

Because the Generic libraries can return errors with for properties not found in the display class there is a version of `CopyErrorsToModelState` that takes a parameter called `displayDto` and it will ensure the name is only set on properties that the `displayDto` has in it.

```csharp
service.Status.CopyErrorsToModelState(ModelState, inputDto);
```

*NOTE: If you are using `CopyErrorsToModelState` with razor pages then you have to provide a prefix to each property name that matches the name of the bound property in the Razor Page C# file. See [this section](https://www.thereformedprogrammer.net/six-things-i-learnt-about-using-asp-net-cores-razor-pages/#changed-the-validation-name-in-the-view-is-a-bit-more-complex) for an explanation on why. Here is an example:*

```csharp
service.Status.CopyErrorsToModelState(ModelState, inputDto, nameof(Data));
```

### 1b. `CopyErrorsToModelState` without a dto

If there is no DTO/ViewModel involved in the service then there is a version that doesn't have a `displayDto` parameter. This leaves the property names left as they were when the error was reported.

```csharp
service.Status.CopyErrorsToModelState(ModelState);
```

## 2. ASP.NET Core Web API - forming the correct HTTP response

If you are using ASP.NET Core Web API you need the status codes of the Generic Libraries turned into the correct HTTP response. The `CreateResponse` static class contains a series of extension methods to do this for you. 

*NOTE: See this [example Web API controller](https://github.com/JonPSmith/EfCore.GenericServices.AspNetCore/blob/master/ExampleWebApi/Controllers/ToDoController.cs) for examples of using the `CreateResponse` extension methods.*

There are the following versions for both the GenericService and GenericBizRunner libraries:

- `IActionResult Response(this IStatusGeneric status)` - this returns the status without any results. 
- `IActionResult ResponseWithValidCode(this IStatusGeneric status, int validStatusCode)` - this returns the status without any results, using the `validStatusCode` if the status has no errors.
- `ActionResult<T> Response<T>(this IStatusGeneric status, T results)` - this returns the status with the results as a json object.
- `ActionResult<T> ResponseWithValidCode<T>(this IStatusGeneric status, T results, int validStatusCode, int nullResultStatusCode = 204)` - this returns the status with the results as a json object, using the `validStatusCode` if the status has no errors and the result isn't null, or if the result is null it returns the `nullResultStatusCode`, which defaults to 204.
- `ActionResult<T> Response(this IStatusGeneric status, ControllerBase controller, string routeName, object routeValues, T dto)` - this can be used with a Create to return a 201 status and a url to obtain the created item

### Return formats

#### 1. Success, no results
The HTTP status code defaults to 200, but you can change this by using the `ResponseWithValidCode` version of the methods. The json sent looks like this:

```json
{
  "message": "Successfully deleted a Todo Item"
}
```

#### 2. Success, with results - result is not null
The HTTP status code defaults to 200, but you can change this by using the `ResponseWithValidCode` version of the methods. The json sent looks like this:

```json
{
  "message": "Success",
  "results": {
    "id": 1,
    "name": "Create ASP.NET Core API project",
    "difficulty": 1
  }
}
```

#### 3. Success, with results where the results is null
The HTTP status code is 204 (NoContent), but you can change this by using the `ResponseWithValidCode` 
version of the method and providing the third parameter, `nullResultStatusCode` with the new status code.
The json sent looks like this:

```json
{
  "message": "The Todo Item was not found."
}
```

#### 4. Create, returning a url
The `ActionResult<T> Response(this IStatusGeneric status, ControllerBase controller, string routeName, object routeValues, T dto)`
method uses Microsoft's `CreatedAtRoute` method to return an HTTP status of 201 (Created), with the url to obtain the created item.
This matches the HTTP status code definitions for a 201 create. Here is an example response:

Response body
```json
{
  "name": "string",
  "difficulty": 1
}
```
Response headers (see returned url on line 3)
```json
 content-type: application/json; charset=utf-8 
 date: Thu, 30 Aug 2018 11:07:48 GMT 
 location: http://localhost:54074/api/ToDo/7 
 ... other parts removed 
```

#### 4. Error
The HTTP status code is 400 (BadRequest). The json sent looks like this:

```json
{
    "": [
        "Global error message"
    ],    
    
    "MyPropery": [
        "The property is required",
        "Another error on the same property"
    ]
}
```

*NOTE: This error format is the one that ASP.NET Core WebAPI when it is set up to validate data on input.*

## Unit test/Integration test of Web APIs

I have added some extension methods in the class [ResponseDecoders](https://github.com/JonPSmith/EfCore.GenericServices.AspNetCore/blob/master/GenericServices.AspNetCore/UnitTesting/ResponseDecoders.cs)
that:

1. Provides you with the HTTP Status Code in the response.
2. Converts Web API responses that were created by the `CreateResponse` extension method into a `GenericServices.IStatusGeneric` type. This allows you to inspect the Status, message, Errors and returned Result.

I have added a [section to my article on Web API](https://www.thereformedprogrammer.net/how-to-write-good-testable-asp-net-core-web-api-code-quickly/?preview=true#update-now-with-unit-test-support) that talks about this feature in more detail. Also do look at [IntegrationTestToDoController](https://github.com/JonPSmith/EfCore.GenericServices.AspNetCore/blob/master/Test/UnitTests/ExampleApp/IntegrationTestToDoController.cs) for an example of how the `ResponseDecoders` extension methods are used.
