using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IEmployerFeedbackContext : IEntityContext<Entities.EmployerFeedback>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public async Task<Entities.EmployerFeedback> GetByUserUkprnAccountAsync(Guid userRef, long ukprn, long accountId, CancellationToken cancellationToken)
            => await Entities.FirstOrDefaultAsync(x => x.UserRef == userRef && x.Ukprn == ukprn && x.AccountId == accountId, cancellationToken);

        public async Task<List<LatestEmployerFeedbackResults>> GetLatestResultsPerAccount(long accountId, Guid userRef, CancellationToken cancellationToken)
        {
            var pre = await Entities
                .Where(ef => ef.AccountId == accountId && ef.UserRef == userRef)
                .SelectMany(ef => ef.FeedbackResults, (ef, er) => new
                {
                    ef.AccountId,
                    ef.Account.AccountName,
                    ef.Ukprn,
                    er.FeedbackId,
                    er.DateTimeCompleted,
                    er.ProviderRating,
                    er.FeedbackSource
                })
                .GroupBy(x => new { x.AccountId, x.Ukprn, x.AccountName })
                .Select(g => new
                {
                    g.Key.AccountId,
                    g.Key.Ukprn,
                    g.Key.AccountName,
                    Latest = g.OrderByDescending(x => x.DateTimeCompleted)
                        .ThenByDescending(x => x.FeedbackId)
                        .Select(x => new
                        {
                            x.DateTimeCompleted,
                            x.ProviderRating,
                            x.FeedbackSource
                        })
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            var results = pre
                .OrderBy(x => x.Ukprn)
                .Select(x => new LatestEmployerFeedbackResults
                {
                    AccountId = x.AccountId,
                    AccountName = x.AccountName,
                    Ukprn = x.Ukprn,
                    DateTimeCompleted = x.Latest?.DateTimeCompleted,
                    ProviderRating = x.Latest?.ProviderRating,
                    FeedbackSource = x.Latest?.FeedbackSource
                })
                .ToList();

            return results;
        }
    }
}
