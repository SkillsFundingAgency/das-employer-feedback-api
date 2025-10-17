using FluentValidation;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpdateFeedbackTransaction
{
    public class UpdateFeedbackTransactionCommandValidator : AbstractValidator<UpdateFeedbackTransactionCommand>
    {
        public UpdateFeedbackTransactionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Id must be provided and greater than 0");

            RuleFor(x => x.TemplateId)
                .NotEmpty()
                .WithMessage("TemplateId must be provided");

            RuleFor(x => x.SentCount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("SentCount must be greater than or equal to 0");

            RuleFor(x => x.SentDate)
                .NotEmpty()
                .WithMessage("SentDate must be provided");
        }
    }
}