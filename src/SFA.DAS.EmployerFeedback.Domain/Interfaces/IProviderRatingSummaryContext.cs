using System.Threading.Tasks;
using SFA.DAS.EmployerFeedback.Domain.Entities;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IProviderRatingSummaryContext : IEntityContext<ProviderRatingSummary>
    {
        Task GenerateFeedbackSummaries();
    }
}