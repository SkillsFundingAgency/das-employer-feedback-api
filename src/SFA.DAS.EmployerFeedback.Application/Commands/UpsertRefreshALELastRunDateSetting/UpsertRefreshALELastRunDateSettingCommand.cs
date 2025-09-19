#nullable enable
using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertRefreshALELastRunDateSetting
{
    public class UpsertRefreshALELastRunDateSettingCommand : IRequest<Unit>
    {
        public string? Value { get; set; }
    }
}
