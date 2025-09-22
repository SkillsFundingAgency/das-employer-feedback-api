using SFA.DAS.EmployerFeedback.Domain.Configuration;
using FluentAssertions;
using AutoFixture.NUnit3;
using NUnit.Framework;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Configuration
{
    public class AzureActiveDirectoryApiConfigurationTests
    {
        [Test, AutoData]
        public void Properties_SetAndGet_ShouldReturnExpectedValues(string tenant, string audiences)
        {
            var config = new AzureActiveDirectoryApiConfiguration
            {
                Tenant = tenant,
                Audiences = audiences
            };
            config.Tenant.Should().Be(tenant);
            config.Audiences.Should().Be(audiences);
        }
    }
}
