using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IProviderAttributeSummaryContext : IEntityContext<ProviderAttributeSummary>
    {
        public async Task<List<ProviderAttributeSummary>> GetProviderAttributeSummaryByUkprnAndTimePeriodAsync(long ukprn, string timePeriod, CancellationToken cancellationToken)
        {
            return await Entities
                .AsNoTracking()
                .Include(x => x.Attribute)
                .Where(x => x.Ukprn == ukprn && x.TimePeriod == timePeriod)
                .ToListAsync(cancellationToken);
        }
    }
}
