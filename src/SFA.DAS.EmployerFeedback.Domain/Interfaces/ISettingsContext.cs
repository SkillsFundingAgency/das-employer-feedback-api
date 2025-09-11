using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface ISettingsContext : IEntityContext<Settings>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        public async Task<Settings> GetByNameAsync(string name, CancellationToken cancellationToken)
            => await Entities.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        public async Task<List<Settings>> GetAll()
            => await Entities.ToListAsync();
    }
}
