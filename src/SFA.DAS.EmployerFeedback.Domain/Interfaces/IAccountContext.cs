using SFA.DAS.EmployerFeedback.Domain.Entities;
using System;
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
            
        public async Task<List<long>> GetAccountsBatchAsync(int batchSize, int batchDays, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-batchDays);
            
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
    }
}