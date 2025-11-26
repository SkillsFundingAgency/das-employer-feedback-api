using SFA.DAS.EmployerFeedback.Domain.Entities;
using System;
using System.Collections.Generic;
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

        public async Task<List<long>> GetFeedbackTransactionsBatchAsync(int batchSize, DateTime currentDateTime, CancellationToken cancellationToken = default)
            => await Entities
                .Where(ft => ft.SendAfter < currentDateTime && ft.SentDate == null)
                .OrderBy(ft => ft.Id)
                .Take(batchSize)
                .Select(ft => ft.Id)
                .ToListAsync(cancellationToken);

        public async Task<FeedbackTransaction> GetByIdWithAccountAsync(long id, CancellationToken cancellationToken = default)
            => await Entities
                .Include(ft => ft.Account)
                .AsNoTracking()
                .FirstOrDefaultAsync(ft => ft.Id == id, cancellationToken);
    }
}