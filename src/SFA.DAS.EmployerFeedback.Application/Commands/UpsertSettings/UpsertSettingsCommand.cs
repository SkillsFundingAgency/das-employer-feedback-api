using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertSettings
{
    public class UpsertSettingsCommand : IRequest<Unit>
    {
        public List<SettingDto> Settings { get; set; }
    }

    public class SettingDto
    {
        public string Name { get; set; }
        public DateTime? Value { get; set; }
    }
}
