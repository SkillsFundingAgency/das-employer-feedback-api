using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetYearlyEmployerFeedbackResult;
using SFA.DAS.EmployerFeedback.Application.Commands.GetYearlyEmployerFeedbackResult;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetYearlyEmployerFeedbackResult
{
    public class GetYearlyEmployerFeedbackResultQueryValidatorTests
    {
        [Test]
        public void Validator_ShouldPass_ForValidUkprn()
        {
            var validator = new GetYearlyEmployerFeedbackResultQueryValidator();
            var query = new GetYearlyEmployerFeedbackResultQuery { Ukprn = 12345 };
            var result = validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(q => q.Ukprn);
        }

        [Test]
        public void Validator_ShouldFail_ForZeroUkprn()
        {
            var validator = new GetYearlyEmployerFeedbackResultQueryValidator();
            var query = new GetYearlyEmployerFeedbackResultQuery { Ukprn = 0 };
            var result = validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.Ukprn);
        }

        [Test]
        public void Validator_ShouldFail_ForNegativeUkprn()
        {
            var validator = new GetYearlyEmployerFeedbackResultQueryValidator();
            var query = new GetYearlyEmployerFeedbackResultQuery { Ukprn = -1 };
            var result = validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.Ukprn);
        }
    }
}
