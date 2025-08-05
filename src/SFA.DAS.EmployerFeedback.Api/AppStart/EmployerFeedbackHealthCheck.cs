using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Api.AppStart
{
    [ExcludeFromCodeCoverage]
    public class EmployerFeedbackHealthCheck : IHealthCheck
    {
        private const string HealthCheckResultsDescription = "Request Employer Feedback API Health Check";
        private readonly IAttributeEntityContext _attributeEntityContext;

        public EmployerFeedbackHealthCheck(IAttributeEntityContext attributeEntityContext)
        {
            _attributeEntityContext = attributeEntityContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var dbConnectionHealthy = true;
            try
            {
                await _attributeEntityContext.GetFirstOrDefault();
            }
            catch
            {
                dbConnectionHealthy = false;
            }

            return dbConnectionHealthy ? HealthCheckResult.Healthy(HealthCheckResultsDescription) : HealthCheckResult.Unhealthy(HealthCheckResultsDescription);
        }
    }
}