using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackRatingsResult;
using SFA.DAS.EmployerFeedback.Application.Commands.GetEmployerFeedbackRatingsResult;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetEmployerFeedbackRatingsResult
{
    public class GetEmployerFeedbackRatingsResultQueryValidatorTests
    {
        [Test]
        public void Validator_ShouldPass_ForValidTimePeriod()
        {
            var validator = new GetEmployerFeedbackRatingsResultQueryValidator();
            var query = new GetEmployerFeedbackRatingsResultQuery { TimePeriod = "AY2023" };
            var result = validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(q => q.TimePeriod);
        }

        [Test]
        public void Validator_ShouldPass_ForEmptyTimePeriod()
        {
            var validator = new GetEmployerFeedbackRatingsResultQueryValidator();
            var query = new GetEmployerFeedbackRatingsResultQuery { TimePeriod = "" };
            var result = validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(q => q.TimePeriod);
        }

        [Test]
        public void Validator_ShouldPass_ForAllTimePeriod()
        {
            var validator = new GetEmployerFeedbackRatingsResultQueryValidator();
            var query = new GetEmployerFeedbackRatingsResultQuery { TimePeriod = "All" };
            var result = validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(q => q.TimePeriod);
        }

        [Test]
        public void Validator_ShouldFail_ForInvalidTimePeriod()
        {
            var validator = new GetEmployerFeedbackRatingsResultQueryValidator();
            var query = new GetEmployerFeedbackRatingsResultQuery { TimePeriod = "2023" };
            var result = validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(q => q.TimePeriod);
        }
    }
}
