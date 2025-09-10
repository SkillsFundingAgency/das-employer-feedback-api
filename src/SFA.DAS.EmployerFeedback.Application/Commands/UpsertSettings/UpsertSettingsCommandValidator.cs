using FluentValidation;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertSettings
{
    public class UpsertSettingsCommandValidator : AbstractValidator<UpsertSettingsCommand>
    {
        public UpsertSettingsCommandValidator()
        {
            RuleFor(x => x.Settings)
                .NotNull().WithMessage("Settings list must not be null.")
                .Must(x => x != null && x.Count > 0).WithMessage("Settings list must not be empty.");
            RuleForEach(x => x.Settings).SetValidator(new SettingDtoValidator());
        }
    }

    public class SettingDtoValidator : AbstractValidator<SettingDto>
    {
        public SettingDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name must not be empty.")
                .Matches("^[A-Za-z0-9_]+$").WithMessage("Name must be alphanumeric or underscore.");

            RuleFor(x => x.Value)
                .NotNull().WithMessage("Value must not be null.")
                .Matches("^[\\w\\s\\-:.,/]+$").WithMessage("Value contains invalid characters.");
        }
    }
}
