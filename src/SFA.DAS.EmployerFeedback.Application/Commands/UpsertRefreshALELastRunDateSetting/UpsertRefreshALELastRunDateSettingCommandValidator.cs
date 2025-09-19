using FluentValidation;
using System;
using System.Globalization;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertRefreshALELastRunDateSetting
{
    public class UpsertRefreshALELastRunDateSettingCommandValidator : AbstractValidator<UpsertRefreshALELastRunDateSettingCommand>
    {
        public UpsertRefreshALELastRunDateSettingCommandValidator()
        {
            RuleFor(x => x.Value)
                .Must(v => string.IsNullOrEmpty(v) || DateTime.TryParse(v, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                .WithMessage("Value must be a valid date string if provided.");
        }
    }
}
