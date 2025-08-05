using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IAttributeEntityContext : IEntityContext<Attributes>
    {
        public async Task<List<Attributes>> GetAll()
                  => await Entities.ToListAsync();
        public async Task<Attributes> GetFirstOrDefault()
            => await Entities
                .FirstOrDefaultAsync();
    }
}
