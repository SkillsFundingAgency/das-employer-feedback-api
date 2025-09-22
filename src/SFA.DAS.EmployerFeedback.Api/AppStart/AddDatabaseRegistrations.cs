using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerFeedback.Data;
using SFA.DAS.EmployerFeedback.Domain.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Api.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AddDatabaseRegistrations
    {
        public static void AddDatabaseRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>();
            if (configuration.IsLocalAcceptanceOrDev())
            {
                services.AddDbContext<EmployerFeedbackDataContext>(options => options.UseSqlServer(appSettings.DbConnectionString).EnableSensitiveDataLogging());
            }
            else if (configuration.IsIntegrationTests())
            {
                services.AddDbContext<EmployerFeedbackDataContext>(options => options.UseSqlServer("Server=localhost;Database=SFA.DAS.EmployerFeedback.IntegrationTests.Database;Trusted_Connection=True;MultipleActiveResultSets=true").EnableSensitiveDataLogging());
            }
            else
            {
                services.AddSingleton(new ChainedTokenCredential(
                    new ManagedIdentityCredential(),
                    new AzureCliCredential(),
                    new VisualStudioCodeCredential(),
                    new VisualStudioCredential())
                );
                services.AddDbContext<EmployerFeedbackDataContext>(ServiceLifetime.Scoped);
            }

            services.AddTransient(provider => new Lazy<EmployerFeedbackDataContext>(provider.GetService<EmployerFeedbackDataContext>()));
        }
    }
}
