using System;
using FluentValidation;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertSettings
{
    public class UpsertRefreshALELastRunDateSettingCommandValidator : AbstractValidator<UpsertRefreshALELastRunDateSettingCommand>
    {
        public UpsertRefreshALELastRunDateSettingCommandValidator()
        {
            RuleFor(x => x.Value)
                .Must(v => string.IsNullOrEmpty(v) || DateTime.TryParse(v, out _))
                .WithMessage("Value must be a valid date string if provided.");
        }
    }
}
