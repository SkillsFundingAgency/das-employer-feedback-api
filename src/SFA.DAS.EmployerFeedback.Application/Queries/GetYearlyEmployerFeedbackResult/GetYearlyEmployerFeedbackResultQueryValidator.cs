using FluentValidation;
using SFA.DAS.EmployerFeedback.Application.Queries.GetYearlyEmployerFeedbackResult;

namespace SFA.DAS.EmployerFeedback.Application.Commands.GetYearlyEmployerFeedbackResult
{
    public class GetYearlyEmployerFeedbackResultQueryValidator : AbstractValidator<GetYearlyEmployerFeedbackResultQuery>
    {
        public GetYearlyEmployerFeedbackResultQueryValidator()
        {
            RuleFor(x => x.Ukprn).GreaterThan(0).WithMessage("Ukprn must be greater than zero.");
        }
    }
}
