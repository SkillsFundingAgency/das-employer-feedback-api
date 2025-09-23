using FluentValidation;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultForAcademicYear;

namespace SFA.DAS.EmployerFeedback.Application.Commands.GetEmployerFeedbackResultForAcademicYear
{
    public class GetEmployerFeedbackResultForAcademicYearQueryValidator : AbstractValidator<GetEmployerFeedbackResultForAcademicYearQuery>
    {
        public GetEmployerFeedbackResultForAcademicYearQueryValidator()
        {
            RuleFor(x => x.Ukprn).GreaterThan(0).WithMessage("Ukprn must be greater than zero.");
            RuleFor(x => x.TimePeriod)
                .NotEmpty().WithMessage("Academic year is required.")
                .Matches("^AY\\d{4}$").WithMessage("Academic year should be in the format 'AYdddd'");
        }
    }
}