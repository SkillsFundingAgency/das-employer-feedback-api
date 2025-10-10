using SFA.DAS.EmployerFeedback.Domain.Configuration;
using FluentAssertions;
using AutoFixture.NUnit3;
using NUnit.Framework;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Configuration
{
    public class ApplicationSettingsTests
    {
        [Test, AutoData]
        public void Properties_SetAndGet_ShouldReturnExpectedValues(string tenant, string audiences, string connStr, int emailNudgeCheckDays)
        {
            var azureAd = new AzureActiveDirectoryApiConfiguration { Tenant = tenant, Audiences = audiences };
            var settings = new ApplicationSettings
            {
                AzureAd = azureAd,
                DbConnectionString = connStr,
                EmailNudgeCheckDays = emailNudgeCheckDays
            };
            settings.AzureAd.Should().Be(azureAd);
            settings.DbConnectionString.Should().Be(connStr);
            settings.EmailNudgeCheckDays.Should().Be(emailNudgeCheckDays);
        }
    }
}
