using FluentValidation;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackStarsResultAnnual;

namespace SFA.DAS.EmployerFeedback.Application.Commands.GetEmployerFeedbackStarsResultAnnual
{
    public class GetEmployerFeedbackStarsResultAnnualQueryValidator : AbstractValidator<GetEmployerFeedbackStarsResultAnnualQuery>
    {
        public GetEmployerFeedbackStarsResultAnnualQueryValidator()
        {
            RuleFor(x => x.TimePeriod)
                .Matches("^(|All|AY\\d{4})$").WithMessage("Time period should be empty, 'All', or in the format 'AYdddd'");
        }
    }
}