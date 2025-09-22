using FluentValidation;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummaryForAcademicYear;

namespace SFA.DAS.EmployerFeedback.Application.Commands.GetEmployerFeedbackResultSummaryForAcademicYear
{
    public class GetEmployerFeedbackResultSummaryForAcademicYearQueryValidator : AbstractValidator<GetEmployerFeedbackResultSummaryForAcademicYearQuery>
    {
        public GetEmployerFeedbackResultSummaryForAcademicYearQueryValidator()
        {
            RuleFor(x => x.Ukprn).GreaterThan(0).WithMessage("Ukprn must be greater than zero.");
            RuleFor(x => x.TimePeriod)
                .NotEmpty().WithMessage("Academic year is required.")
                .Matches("^AY\\d{4}$").WithMessage("Academic year should be in the format 'AYdddd'");
        }
    }
}