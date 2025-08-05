using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Api.Authorization
{
    [ExcludeFromCodeCoverage]
    public class LocalAuthorizationHandler : AuthorizationHandler<NoneRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            NoneRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
