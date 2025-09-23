using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultForAcademicYear;
using SFA.DAS.EmployerFeedback.Application.Commands.GetEmployerFeedbackResultForAcademicYear;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetEmployerFeedbackResultForAcademicYear
{
    public class GetEmployerFeedbackResultForAcademicYearQueryValidatorTests
    {
        [Test]
        public void Validator_ShouldPass_ForValidQuery()
        {
            var validator = new GetEmployerFeedbackResultForAcademicYearQueryValidator();
            var query = new GetEmployerFeedbackResultForAcademicYearQuery { Ukprn = 12345, TimePeriod = "AY2023" };
            var result = validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(q => q.Ukprn);
            result.ShouldNotHaveValidationErrorFor(q => q.TimePeriod);
        }

        [Test]
        public void Validator_ShouldFail_ForZeroUkprn()
        {
            var validator = new GetEmployerFeedbackResultForAcademicYearQueryValidator();
            var query = new GetEmployerFeedbackResultForAcademicYearQuery { Ukprn = 0, TimePeriod = "AY2023" };
            var result = validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.Ukprn);
        }

        [Test]
        public void Validator_ShouldFail_ForNegativeUkprn()
        {
            var validator = new GetEmployerFeedbackResultForAcademicYearQueryValidator();
            var query = new GetEmployerFeedbackResultForAcademicYearQuery { Ukprn = -1, TimePeriod = "AY2023" };
            var result = validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.Ukprn);
        }

        [Test]
        public void Validator_ShouldFail_ForEmptyTimePeriod()
        {
            var validator = new GetEmployerFeedbackResultForAcademicYearQueryValidator();
            var query = new GetEmployerFeedbackResultForAcademicYearQuery { Ukprn = 12345, TimePeriod = "" };
            var result = validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.TimePeriod);
        }

        [Test]
        public void Validator_ShouldFail_ForInvalidTimePeriodFormat()
        {
            var validator = new GetEmployerFeedbackResultForAcademicYearQueryValidator();
            var query = new GetEmployerFeedbackResultForAcademicYearQuery { Ukprn = 12345, TimePeriod = "2023" };
            var result = validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.TimePeriod);
        }
    }
}
