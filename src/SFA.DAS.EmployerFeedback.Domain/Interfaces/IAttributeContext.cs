using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IAttributeContext : IEntityContext<Attribute>
    {
        public async Task<List<Attribute>> GetAll()
                  => await Entities.ToListAsync();
        public async Task<Attribute> GetFirstOrDefault()
            => await Entities
                .FirstOrDefaultAsync();
    }
}
