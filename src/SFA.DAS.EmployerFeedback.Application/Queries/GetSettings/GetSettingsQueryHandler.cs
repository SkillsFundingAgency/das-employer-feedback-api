using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerFeedback.Application.Models;

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
            var setting = await _settingsContext.GetByNameAsync(SettingType.RefreshALELastRunDate.ToString(), cancellationToken);
            return new GetSettingsQueryResult { Value = setting?.Value };
        }
    }
}
