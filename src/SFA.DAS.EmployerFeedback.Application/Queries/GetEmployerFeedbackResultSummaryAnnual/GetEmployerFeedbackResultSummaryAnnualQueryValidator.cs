using FluentValidation;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummaryAnnual;

namespace SFA.DAS.EmployerFeedback.Application.Commands.GetEmployerFeedbackResultSummaryAnnual
{
    public class GetEmployerFeedbackResultSummaryAnnualQueryValidator : AbstractValidator<GetEmployerFeedbackResultSummaryAnnualQuery>
    {
        public GetEmployerFeedbackResultSummaryAnnualQueryValidator()
        {
            RuleFor(x => x.Ukprn).GreaterThan(0).WithMessage("Ukprn must be greater than zero.");
        }
    }
}