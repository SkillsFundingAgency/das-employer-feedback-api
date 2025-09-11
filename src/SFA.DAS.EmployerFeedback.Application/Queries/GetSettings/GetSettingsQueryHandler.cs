using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetSettings
{
    public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, GetSettingsQueryResult>
    {
        private readonly ISettingsContext _settingsContext;

        public GetSettingsQueryHandler(ISettingsContext settingsContext)
        {
            _settingsContext = settingsContext;
        }

        public async Task<GetSettingsQueryResult> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
        {
            var settings = await _settingsContext.GetAll();
            return new GetSettingsQueryResult
            {
                Settings = settings.Select(s => new Settings { Name = s.Name, Value = s.Value }).ToList()
            };
        }
    }
}
