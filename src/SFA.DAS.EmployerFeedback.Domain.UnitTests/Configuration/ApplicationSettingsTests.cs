using SFA.DAS.EmployerFeedback.Domain.Configuration;
using FluentAssertions;
using AutoFixture.NUnit3;
using NUnit.Framework;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Configuration
{
    public class ApplicationSettingsTests
    {
        [Test, AutoData]
        public void Properties_SetAndGet_ShouldReturnExpectedValues(string tenant, string audiences, string connStr, int batchDays)
        {
            var azureAd = new AzureActiveDirectoryApiConfiguration { Tenant = tenant, Audiences = audiences };
            var settings = new ApplicationSettings
            {
                AzureAd = azureAd,
                DbConnectionString = connStr,
                BatchDays = batchDays
            };
            settings.AzureAd.Should().Be(azureAd);
            settings.DbConnectionString.Should().Be(connStr);
            settings.BatchDays.Should().Be(batchDays);
        }
    }
}
