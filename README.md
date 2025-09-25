## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Employer Feedback API 
<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/das-employer-feedback-api?repoName=SkillsFundingAgency%2Fdas-employer-feedback-api&branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build?definitionId=4150)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-employer-feedback-api&metric=alert_status)](https://sonarcloud.io/project/overview?id=SkillsFundingAgency_das-employer-feedback-api)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/browse/P2-2796)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3773497345/Employer+Feedback+-+QF)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

This repository represents the Employer Feedback API code base. Employer Feedback is a service that allows employer users to provide feedback on their employees apprenticeship training. The employer is able to submit feedback via the ad hoc journey, or an emailing journey. Either way, the UI code base is the `das-employer-feedback-web` repository, this repository is the inner API, and the outer API code base is in the `das-apim-endpoints` repository within the `EmployerFeedback` project.

## Developer Setup
### Requirements

In order to run this solution locally you will need the following:

* A clone of this repository
* **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
* A code editor that supports .NET 8.0 (e.g., Visual Studio 2022, VS Code with C# Dev Kit)
* **SQL Server** - Local SQL Server instance (e.g., SQL Server 2022 Developer Edition, SQL Server Express LocalDB)
* **Azurite** - For local Azure Storage emulation ([Installation guide](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite))

### Environment setup

* **Database** - Publish the local database from the `SFA.DAS.EmployerFeedback.Database` project. 
* **appsettings.development.json file** - Add the following to an `appsettings.development.json` file.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true",
  "ConfigNames": "SFA.DAS.EmployerFeedback.Api",
  "EnvironmentName": "LOCAL",
  "Version": "1.0",
  "APPLICATIONINSIGHTS_CONNECTION_STRING": ""
} 
```

* **Azure Table Storage Config** - Add the following data to your Azure Table Storage Config. 

Row Key: SFA.DAS.EmployerFeedback.Api_1.0

Partition Key: LOCAL

Data:

```
{
  "ApplicationSettings": {
    "DbConnectionString": "Data Source=.;Initial Catalog=SFA.DAS.EmployerFeedback.Database;Integrated Security=True;Pooling=False;Connect Timeout=30;TrustServerCertificate=True;",
    "AzureAd": {
      "Tenant": "citizenazuresfabisgov.onmicrosoft.com",
      "Audiences": "https://citizenazuresfabisgov.onmicrosoft.com/das-at-empfbapi-as-ar"
    }
  } 
}
```

### Running

* Start Azurite 
* Run the solution
* NB: You may need other applications running in conjunction with this, such as the backend API `das-apim-endpoints/EmployerFeedback` project and also the UI `das-employer-feedback-web` codebase for the UI journey.

### Tests

This codebase includes unit tests, which are organized into separate projects. Each project is appropriately named and located in the `Tests` folder. 

#### Unit Tests

There are several unit test projects in the solution built using C#, NUnit, Moq, FluentAssertions and AutoFixture.
* `SFA.DAS.EmployerFeedback.Api.UnitTests`
* `SFA.DAS.EmployerFeedback.Application.UnitTests`
* `SFA.DAS.EmployerFeedback.Domain.UnitTests`


