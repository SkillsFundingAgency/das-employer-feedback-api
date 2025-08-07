using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerFeedback.Application.Behaviours;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Behaviours
{
    public class ValidationBehaviourTests
    {
        public class DummyRequest : IRequest<string> { }

        private class CallTracker
        {
            public bool Called { get; set; }
        }

        private RequestHandlerDelegate<string> MakeDelegate(CallTracker tracker)
        {
            return (CancellationToken ct) =>
            {
                tracker.Called = true;
                return Task.FromResult("ok");
            };
        }

        [Test]
        public async Task Handle_NoValidators_CallsNext()
        {
            var behaviour = new ValidationBehaviour<DummyRequest, string>(Enumerable.Empty<IValidator<DummyRequest>>());

            var tracker = new CallTracker();

            var result = await behaviour.Handle(new DummyRequest(), MakeDelegate(tracker), CancellationToken.None);

            tracker.Called.Should().BeTrue();
            result.Should().Be("ok");
        }

        [Test]
        public void Handle_WithValidationFailures_ThrowsValidationException()
        {
            var validator = new Mock<IValidator<DummyRequest>>();
            var failures = new[] { new ValidationFailure("prop", "error") };

            validator.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(failures));

            var behaviour = new ValidationBehaviour<DummyRequest, string>(new[] { validator.Object });

            var tracker = new CallTracker();

            var ex = Assert.ThrowsAsync<ValidationException>(async () =>
                await behaviour.Handle(new DummyRequest(), MakeDelegate(tracker), CancellationToken.None));

            ex.Errors.Should().Contain(failures);
            tracker.Called.Should().BeFalse();
        }

        [Test]
        public async Task Handle_WithNoFailures_CallsNext()
        {
            var validator = new Mock<IValidator<DummyRequest>>();

            validator.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var behaviour = new ValidationBehaviour<DummyRequest, string>(new[] { validator.Object });

            var tracker = new CallTracker();

            var result = await behaviour.Handle(new DummyRequest(), MakeDelegate(tracker), CancellationToken.None);

            tracker.Called.Should().BeTrue();
            result.Should().Be("ok");
        }
    }
}
