using MediatR;
using System;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertSettings
{
    public class UpsertSettingsCommand : IRequest<Unit>
    {
        public DateTime? Value { get; set; }
    }
}
