using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertSettings
{
    public class UpsertSettingsCommand : IRequest<Unit>
    {
        public List<SettingDto> Settings { get; set; }
    }

    public class SettingDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
