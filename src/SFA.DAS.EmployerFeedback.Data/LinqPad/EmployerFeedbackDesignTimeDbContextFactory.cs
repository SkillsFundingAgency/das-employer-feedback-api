using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Data.LinqPad
{
    [ExcludeFromCodeCoverage]
    public class EmployerFeedbackDesignTimeDataContext : EmployerFeedbackDataContext
    {
        public EmployerFeedbackDesignTimeDataContext(string connectionString)
            : base(new DbContextOptionsBuilder<EmployerFeedbackDataContext>()
                    .UseSqlServer(connectionString)
                    .AddInterceptors(new AzureAdTokenInterceptor())
                    .Options)
        {
        }
    }
}

