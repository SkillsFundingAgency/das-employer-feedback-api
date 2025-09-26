using Microsoft.EntityFrameworkCore;
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
                .Where(eft => eft.AccountId == accountId && eft.UserRef == userRef)
                .Select(eft => new
                {
                    eft.AccountId,
                    eft.Ukprn,
                    eft.Account.AccountName,
                    Latest = eft.FeedbackResults
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
        public async Task<List<AllEmployerFeedbackResults>> GetAllEmployerFeedbackAsync(CancellationToken cancellationToken)
        {
            var groupedFeedback = await Entities
                .AsNoTracking()
                .SelectMany(e => e.FeedbackResults, (feedback, result) => new
                {
                    feedback.Ukprn,
                    result.Id,
                    result.DateTimeCompleted,
                    result.ProviderRating,
                    Attributes = result.ProviderAttributes.Select(pa => new
                    {
                        AttributeName = pa.Attribute.AttributeName,
                        pa.AttributeValue
                    })
                })
                .GroupBy(x => new { x.Id, x.Ukprn, x.DateTimeCompleted, x.ProviderRating })
                .Select(g => new AllEmployerFeedbackResults
                {
                    Ukprn = g.Key.Ukprn,
                    DateTimeCompleted = g.Key.DateTimeCompleted,
                    ProviderRating = g.Key.ProviderRating,
                    ProviderAttributes = g
                        .SelectMany(x => x.Attributes)
                        .Where(a => a.AttributeName != null)
                        .Select(a => new ProviderAttributeResults
                        {
                            Name = a.AttributeName,
                            Value = a.AttributeValue
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            return groupedFeedback;
        }
    }
}
