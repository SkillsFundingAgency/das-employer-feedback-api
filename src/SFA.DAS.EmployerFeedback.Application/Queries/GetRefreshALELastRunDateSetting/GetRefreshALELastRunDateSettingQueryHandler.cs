using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerFeedback.Application.Models;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetRefreshALELastRunDateSetting
{
    public class GetRefreshALELastRunDateSettingQueryHandler : IRequestHandler<GetRefreshALELastRunDateSettingQuery, GetRefreshALELastRunDateSettingQueryResult>
    {
        private readonly ISettingsContext _settingsContext;

        public GetRefreshALELastRunDateSettingQueryHandler(ISettingsContext settingsContext)
        {
            _settingsContext = settingsContext;
        }

        public async Task<GetRefreshALELastRunDateSettingQueryResult> Handle(GetRefreshALELastRunDateSettingQuery request, CancellationToken cancellationToken)
        {
            var setting = await _settingsContext.GetByNameAsync(SettingType.RefreshALELastRunDate.ToString(), cancellationToken);
            return new GetRefreshALELastRunDateSettingQueryResult { Value = setting?.Value };
        }
    }
}
