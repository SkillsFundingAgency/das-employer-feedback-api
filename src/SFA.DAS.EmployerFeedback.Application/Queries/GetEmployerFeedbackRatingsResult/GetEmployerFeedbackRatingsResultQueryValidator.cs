using FluentValidation;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackRatingsResult;

namespace SFA.DAS.EmployerFeedback.Application.Commands.GetEmployerFeedbackRatingsResult
{
    public class GetEmployerFeedbackRatingsResultQueryValidator : AbstractValidator<GetEmployerFeedbackRatingsResultQuery>
    {
        public GetEmployerFeedbackRatingsResultQueryValidator()
        {
            RuleFor(x => x.TimePeriod)
                .Matches("^(|All|AY\\d{4})$").WithMessage("Time period should be empty, 'All', or in the format 'AYdddd'");
        }
    }
}