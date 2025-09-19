using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IAccountContext : IEntityContext<Account>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public async Task<List<Account>> GetAccountsByIdsAsync(IEnumerable<long> accountIds, CancellationToken cancellationToken)
            => await Entities.Where(a => accountIds.Contains(a.Id)).ToListAsync(cancellationToken);
    }
}