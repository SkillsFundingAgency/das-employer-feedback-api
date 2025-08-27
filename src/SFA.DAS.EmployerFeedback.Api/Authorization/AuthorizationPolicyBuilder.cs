using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Api.Authorization
{
    [ExcludeFromCodeCoverage]
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder AllowAnonymousUser(this AuthorizationPolicyBuilder builder)
        {
            builder.Requirements.Add(new NoneRequirement());
            return builder;
        }
    }
}
