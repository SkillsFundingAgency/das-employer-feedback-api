using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Api.Authorization
{
    [ExcludeFromCodeCoverage]
    public class NoneRequirement : IAuthorizationRequirement
    {
    }
}
