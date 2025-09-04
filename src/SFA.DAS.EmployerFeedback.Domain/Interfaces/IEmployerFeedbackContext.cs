using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IEmployerFeedbackContext : IEntityContext<Entities.EmployerFeedback>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public async Task<Entities.EmployerFeedback> GetByUserUkprnAccountAsync(Guid userRef, long ukprn, long accountId, CancellationToken cancellationToken)
            => await Entities.FirstOrDefaultAsync(x => x.UserRef == userRef && x.Ukprn == ukprn && x.AccountId == accountId, cancellationToken);
    }
}
