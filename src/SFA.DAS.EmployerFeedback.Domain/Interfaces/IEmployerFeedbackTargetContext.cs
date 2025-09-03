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
    public interface IEmployerFeedbackTargetContext : IEntityContext<EmployerFeedbackTarget>
    {
        public async Task<List<LatestEmployerFeedbackResults>> GetLatestResultsPerAccount(long accountId, Guid userRef, CancellationToken cancellationToken)
        {
            var pre = await Entities
                .Where(eft => eft.AccountId == accountId && eft.UserRef == userRef)
                .Select(eft => new
                {
                    eft.AccountId,
                    eft.Ukprn,
                    AccountName = eft.Account.Name,
                    Latest = eft.EmployerFeedbackResults
                        .OrderByDescending(r => r.DateTimeCompleted)
                        .Select(r => new
                        {
                            r.DateTimeCompleted,
                            r.ProviderRating,
                            r.FeedbackSource
                        })
                        .FirstOrDefault()
                })
                .GroupBy(x => new { x.AccountId, x.Ukprn, x.AccountName })
                .Select(g => new
                {
                    g.Key.AccountId,
                    g.Key.Ukprn,
                    g.Key.AccountName,
                    Top = g.OrderByDescending(x => x.Latest != null)
                        .ThenByDescending(x => x.Latest!.DateTimeCompleted)
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
                    DateTimeCompleted = x.Top?.Latest?.DateTimeCompleted,
                    ProviderRating = x.Top?.Latest?.ProviderRating,
                    FeedbackSource = x.Top?.Latest?.FeedbackSource
                })
                .ToList();

            return results;
        }
    }
}