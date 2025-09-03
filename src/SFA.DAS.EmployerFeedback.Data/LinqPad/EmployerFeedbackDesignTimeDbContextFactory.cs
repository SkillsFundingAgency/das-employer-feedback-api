using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.EmployerFeedback.Data.LinqPad
{
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

