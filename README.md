RSMFValidatorSampleCode
=======

## Introduction

The RSMFValidatorSampleCode project illustrates how to leverage the RSMF Validator library provided by Relativity.  The RSMF JSON schema is included in the RSMFManifestSchema directory.

## Build

To build the sample code, open the RSMFValidatorSampleCode.sln in Visual Studio 2015.  Restore the Nuget packages which will download and install the RSMF Validator library.  Build in release or debug mode.

## RSMF sample files

A short walkthrough with invalid RSMF sample files is included in the Documentation directory.  See RSMFValidatorSampleCode.pdf.

## RSMF validator application

An RSMF validator application is provided in the 'bin' directory.  It will be used by the examples in RSMFValidatorSampleCode.pdf.  It may also be used to validate RSMF files that are causing issues in production.
It has a 'Save Error Report' feature that can be used to send information to Relativity support.  Please be advised that the report may contain personal information, so it's best to scrub any PII from the report
before sending it to Relativity.