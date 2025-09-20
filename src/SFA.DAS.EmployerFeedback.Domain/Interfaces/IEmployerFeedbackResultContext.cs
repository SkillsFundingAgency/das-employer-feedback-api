using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IEmployerFeedbackResultContext : IEntityContext<EmployerFeedbackResult>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
