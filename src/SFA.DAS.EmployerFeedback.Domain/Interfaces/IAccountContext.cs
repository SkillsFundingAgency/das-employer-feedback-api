using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Models;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IAccountContext : IEntityContext<Account>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public async Task<Account> GetByIdAsync(long id, CancellationToken cancellationToken = default)
            => await Entities.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        public async Task<List<Account>> GetAccountsByIdsAsync(IEnumerable<long> accountIds, CancellationToken cancellationToken)
            => await Entities.Where(a => accountIds.Contains(a.Id)).ToListAsync(cancellationToken);
        public async Task<List<long>> GetEmailNudgeAccountsBatchAsync(int batchSize, int emailNudgeCheckDays, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-emailNudgeCheckDays);
            var accountIds = await Entities
                .Where(a => a.CheckedOn == null || a.CheckedOn < cutoffDate)
                .OrderBy(a => a.CheckedOn.HasValue ? 1 : 0)
                .ThenBy(a => a.CheckedOn)
                .ThenBy(a => a.Id)
                .Take(batchSize)
                .Select(a => a.Id)
                .ToListAsync(cancellationToken);

            return accountIds;
        }

        public async Task<List<LatestEmployerFeedbackResults>> GetLatestResultsPerAccount(long accountId, Guid userRef, CancellationToken cancellationToken)
        {
            var pre = await Entities
                .Where(a => a.Id == accountId)
                .Select(a => new
                {
                    a.Id,
                    a.AccountName,
                    Feedbacks = a.EmployerFeedbacks
                        .Where(ef => ef.UserRef == userRef)
                        .Select(ef => new
                        {
                            ef.Ukprn,
                            Results = ef.FeedbackResults
                                .Select(fr => new
                                {
                                    fr.FeedbackId,
                                    fr.DateTimeCompleted,
                                    fr.ProviderRating,
                                    fr.FeedbackSource
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (pre == null)
                return new List<LatestEmployerFeedbackResults>();

            // keep only feedbacks that have results and a ukprn
            var perUkprn = pre.Feedbacks
                .Where(ef => ef.Results != null && ef.Results.Any())
                .GroupBy(ef => (long?)ef.Ukprn)
                .Select(g =>
                {
                    var latest = g.SelectMany(x => x.Results)
                        .OrderByDescending(r => r.DateTimeCompleted)
                        .ThenByDescending(r => r.FeedbackId)
                        .FirstOrDefault();

                    return new LatestEmployerFeedbackResults
                    {
                        AccountId = pre.Id,
                        AccountName = pre.AccountName,
                        Ukprn = g.Key,
                        DateTimeCompleted = latest?.DateTimeCompleted,
                        ProviderRating = latest?.ProviderRating,
                        FeedbackSource = latest?.FeedbackSource
                    };
                })
                .OrderBy(x => x.Ukprn)
                .ToList();

            // if no valid feedbacks for this user return a placeholder row for the account
            if (perUkprn.Count == 0)
            {
                return new List<LatestEmployerFeedbackResults>
                {
                    new LatestEmployerFeedbackResults
                    {
                        AccountId = pre.Id,
                        AccountName = pre.AccountName,
                        Ukprn = null,
                        DateTimeCompleted = null,
                        ProviderRating = null,
                        FeedbackSource = null
                    }
                };
            }

            return perUkprn;
        }
    }
}