# EfCore.GenericServices.AspNetCore

Contains code to do two things:

1. `CopyErrorsToModelState` which convert GenericServices' `IStatusGeneric` errors into ASP.NET Core's `ModelState`.
Useful when working html/razor pages.
2. `Response` which turns a GenericService `IStatusGeneric` into a Web API formatted return.
   * HTTP 200 (OK) with a result and the status Message string
   * HTTP 404 (not found) if data was null
   * HTTP 400 (invalid format) with errors in the same format as Web API uses.


MIT licence