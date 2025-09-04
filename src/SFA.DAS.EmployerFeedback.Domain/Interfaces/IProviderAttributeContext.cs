using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IProviderAttributeContext : IEntityContext<ProviderAttribute>
    {
        Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default);
    }
}
