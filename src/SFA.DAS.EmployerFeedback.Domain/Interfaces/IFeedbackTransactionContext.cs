using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IFeedbackTransactionContext : IEntityContext<FeedbackTransaction>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        public async Task<FeedbackTransaction> GetMostRecentByAccountIdAsync(long accountId, CancellationToken cancellationToken = default)
            => await Entities
                .Where(ft => ft.AccountId == accountId)
                .OrderByDescending(ft => ft.CreatedOn)
                .FirstOrDefaultAsync(cancellationToken);
    }
}