using FluentValidation;

namespace SFA.DAS.EmployerFeedback.Application.Commands.SubmitEmployerFeedback
{
    public class SubmitEmployerFeedbackCommandValidator : AbstractValidator<SubmitEmployerFeedbackCommand>
    {
        public SubmitEmployerFeedbackCommandValidator()
        {
            RuleFor(x => x.UserRef).NotEmpty().WithMessage("UserRef must not be empty.");
            RuleFor(x => x.Ukprn).GreaterThan(0).WithMessage("Ukprn must be greater than zero.");
            RuleFor(x => x.AccountId).GreaterThan(0).WithMessage("Account Id must be greater than zero.");
            RuleFor(x => x.ProviderRating).IsInEnum().WithMessage("ProviderRating must be a valid enum value.");
            RuleFor(x => x.FeedbackSource)
                .InclusiveBetween(1, 2)
                .WithMessage("FeedbackSource must be 1 or 2.");
            RuleForEach(x => x.ProviderAttributes).SetValidator(new ProviderAttributeDtoValidator());
        }
    }

    public class ProviderAttributeDtoValidator : AbstractValidator<ProviderAttributeDto>
    {
        public ProviderAttributeDtoValidator()
        {
            RuleFor(x => x.AttributeId)
                .GreaterThan(0)
                .WithMessage("AttributeId must be greater than zero.");

            RuleFor(x => x.AttributeValue)
                .Must(v => v == 1 || v == -1)
                .WithMessage("AttributeValue must be either 1 or -1.");
        }
    }
}