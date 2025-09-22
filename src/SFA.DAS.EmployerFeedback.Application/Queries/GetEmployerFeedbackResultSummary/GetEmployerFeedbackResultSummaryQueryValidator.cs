using FluentValidation;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummary;

namespace SFA.DAS.EmployerFeedback.Application.Commands.GetEmployerFeedbackResultSummary
{
    public class GetEmployerFeedbackResultSummaryQueryValidator : AbstractValidator<GetEmployerFeedbackResultSummaryQuery>
    {
        public GetEmployerFeedbackResultSummaryQueryValidator()
        {
            RuleFor(x => x.Ukprn).GreaterThan(0).WithMessage("Ukprn must be greater than zero.");
        }
    }
}