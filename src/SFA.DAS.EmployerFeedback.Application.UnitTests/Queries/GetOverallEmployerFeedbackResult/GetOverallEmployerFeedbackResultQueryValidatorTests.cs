using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetOverallEmployerFeedbackResult;
using SFA.DAS.EmployerFeedback.Application.Commands.GetOverallEmployerFeedbackResult;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetOverallEmployerFeedbackResult
{
    public class GetOverallEmployerFeedbackResultQueryValidatorTests
    {
        [Test]
        public void Validator_ShouldPass_ForValidUkprn()
        {
            var validator = new GetOverallEmployerFeedbackResultQueryValidator();
            var query = new GetOverallEmployerFeedbackResultQuery { Ukprn = 12345 };
            var result = validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(q => q.Ukprn);
        }

        [Test]
        public void Validator_ShouldFail_ForZeroUkprn()
        {
            var validator = new GetOverallEmployerFeedbackResultQueryValidator();
            var query = new GetOverallEmployerFeedbackResultQuery { Ukprn = 0 };
            var result = validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.Ukprn);
        }

        [Test]
        public void Validator_ShouldFail_ForNegativeUkprn()
        {
            var validator = new GetOverallEmployerFeedbackResultQueryValidator();
            var query = new GetOverallEmployerFeedbackResultQuery { Ukprn = -1 };
            var result = validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.Ukprn);
        }
    }
}
