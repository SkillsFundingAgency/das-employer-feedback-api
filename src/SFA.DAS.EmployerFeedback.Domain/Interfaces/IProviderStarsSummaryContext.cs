using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IProviderStarsSummaryContext : IEntityContext<ProviderStarsSummary>
    {
        public async Task<List<ProviderStarsSummary>> GetProviderStarsSummaryByUkprnAndTimePeriodAsync(long ukprn, string timePeriod, CancellationToken cancellationToken)
        {
            return await Entities
                 .AsNoTracking()
                .Where(x => x.Ukprn == ukprn && x.TimePeriod == timePeriod)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ProviderStarsSummary>> GetProviderStarsSummaryByUkprnAsync(long ukprn, CancellationToken cancellationToken)
        {
            return await Entities
                 .AsNoTracking()
                .Where(x => x.Ukprn == ukprn)
                .ToListAsync(cancellationToken);
        }
    }
}
