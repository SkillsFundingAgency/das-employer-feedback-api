using FluentValidation;
using SFA.DAS.EmployerFeedback.Application.Queries.GetOverallEmployerFeedbackResult;

namespace SFA.DAS.EmployerFeedback.Application.Commands.GetOverallEmployerFeedbackResult
{
    public class GetOverallEmployerFeedbackResultQueryValidator : AbstractValidator<GetOverallEmployerFeedbackResultQuery>
    {
        public GetOverallEmployerFeedbackResultQueryValidator()
        {
            RuleFor(x => x.Ukprn).GreaterThan(0).WithMessage("Ukprn must be greater than zero.");
        }
    }
}