using FluentValidation;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertFeedbackTransaction
{
    public class UpsertFeedbackTransactionCommandValidator : AbstractValidator<UpsertFeedbackTransactionCommand>
    {
        public UpsertFeedbackTransactionCommandValidator()
        {
            RuleFor(x => x.AccountId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Account ID must be provided and greater than 0");

            RuleForEach(x => x.Active).ChildRules(provider =>
            {
                provider.RuleFor(x => x.Ukprn)
                    .NotEmpty()
                    .GreaterThan(0)
                    .WithMessage("UKPRN must be provided and greater than 0");

                provider.RuleFor(x => x.CourseCode)
                    .NotEmpty()
                    .WithMessage("Course code must be provided");
            });

            RuleForEach(x => x.Completed).ChildRules(provider =>
            {
                provider.RuleFor(x => x.Ukprn)
                    .NotEmpty()
                    .GreaterThan(0)
                    .WithMessage("UKPRN must be provided and greater than 0");

                provider.RuleFor(x => x.CourseCode)
                    .NotEmpty()
                    .WithMessage("Course code must be provided");
            });

            RuleForEach(x => x.NewStart).ChildRules(provider =>
            {
                provider.RuleFor(x => x.Ukprn)
                    .NotEmpty()
                    .GreaterThan(0)
                    .WithMessage("UKPRN must be provided and greater than 0");

                provider.RuleFor(x => x.CourseCode)
                    .NotEmpty()
                    .WithMessage("Course code must be provided");
            });
        }
    }
}