# Release Notes

## 4.1.1

- Bug fix: Changed to Microsoft.AspNetCore.Mvc.Core to solve issue #4.

## 4.1.0

- Update: Now uses StatusGeneric for status class. This supports the new versions of EfCore.GenericServices, EfCorr.GenericBizRunner, and EfCore.GenericEventRunner.

## 4.0.0

- Support both EF Core >=2.1 and EF Core >=3.0 by supporting NetStandard2.0 and NetStandard2.1.
- Updated with latest EfCore.GenericServices and EfCore.GenericBizrunner

## 3.0.2

- Minor change: CheckCreateResponse now returns list of validation errors, not a line saying there was one error.

## 3.0.1

- New Feature: Web API now supports CreatedAtRoute return

## 3.0.0

- Breaking change: Changed the returned ActionResult{T} type to fully define the return - makes swagger definition correct
     - Breaking change: Now returns 204 on null result to be in line with common practices (was 404)

## 2.0.0

- Breaking change: Now has specific typed class for Web API, so that it is easier to unit test
- New Feature: Added ResponseDecoder to make testing easier of Web API action methods that uses the CreateResponse to return a result
- New feature: Added second parameter to `ResponseWithValidCode` to allow the StatusCode for null return to be set - default is NoFound (HTTP 404)

## 1.2.1

- New feature: Added `ResponseWithValidCode` methods to allow the StatusCode to be set for the success path - useful for things like 'POST' where you might want to return a status code of 201 instead of 200
- Updated to newest versions of GenericServices and GenericBizRunner