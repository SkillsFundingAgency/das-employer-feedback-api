using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Models;
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

        public async Task<FeedbackTransactionSummary> GetMostRecentSummaryByAccountIdAsync(long accountId, CancellationToken cancellationToken = default)
            => await Entities
                .AsNoTracking()
                .Where(ft => ft.AccountId == accountId)
                .OrderByDescending(ft => ft.Id)
                .Select(ft => new FeedbackTransactionSummary
                {
                    Id = ft.Id,
                    AccountId = ft.AccountId,
                    SendAfter = ft.SendAfter,
                    SentDate = ft.SentDate
                })
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

        public async Task<FeedbackTransaction> GetByIdAsync(long id, CancellationToken cancellationToken = default)
            => await Entities
                .FirstOrDefaultAsync(ft => ft.Id == id, cancellationToken);
    }
}