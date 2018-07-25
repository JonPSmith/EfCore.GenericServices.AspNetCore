# EfCore.GenericServices.AspNetCore

This library provides converters from [EfCore.GenericService](https://github.com/JonPSmith/EfCore.GenericServices)
and [EfCore.GenericBizRunner](https://github.com/JonPSmith/EfCore.GenericBizRunner) status results to
into two ASP.NET Core formats. 


1. `CopyErrorsToModelState` which convert `IStatusGeneric` errors into ASP.NET Core's `ModelState`.
Useful when working html/razor pages.
2. `Response` which turns a `IStatusGeneric`, with optional result, into a Web API formatted return.
   * HTTP 200 (OK) with a result and the status Message string
   * HTTP 404 (Not found) if data was null
   * HTTP 400 (Bad request) with errors in the same format as Web API uses.

## `Response` formats

- Valid status, i.e. no errors:  HTTP 200 (OK) with json result with properly `message` containing the Message sent back by GenericServices/GenericBizRunner
- Status has errors: HTTP 400 (Bad request) with errors in Web API format (see below)
- Valid status, with result:
   - Results is null: HTTP 404 (Not Found) with json result with properly `message` containing the Message sent back by GenericServices/GenericBizRunner
   - Results is not null:   HTTP 200 (OK) with json result with properly `message` containing the Message, and `results` containing the results

### Web API error format

If using Web API in ASP.NET Core 2.1 or higher the default action is to validate the input to the action
and return a HTTP 400 (Bad request) with the errors send as a dictionary format in json, with the name of 
the property that had the error and an array of the error messages for that property.
See example below:

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

The `Response` extension method uses that format to return `IStatusGeneric` so that you have a common error response format.

MIT licence