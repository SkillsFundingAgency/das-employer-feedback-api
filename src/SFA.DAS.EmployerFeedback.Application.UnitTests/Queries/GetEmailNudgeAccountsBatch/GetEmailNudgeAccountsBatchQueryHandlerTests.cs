using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmailNudgeAccountsBatch;
using SFA.DAS.EmployerFeedback.Domain.Configuration;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetEmailNudgeAccountsBatch
{
    [TestFixture]
    public class GetEmailNudgeAccountsBatchQueryHandlerTests
    {
        private Mock<IAccountContext> _accountContext;
        private Mock<IOptions<ApplicationSettings>> _applicationSettings;
        private GetEmailNudgeAccountsBatchQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _accountContext = new Mock<IAccountContext>();
            _applicationSettings = new Mock<IOptions<ApplicationSettings>>();
            _applicationSettings.Setup(x => x.Value).Returns(new ApplicationSettings { EmailNudgeCheckDays = 30 });
            _handler = new GetEmailNudgeAccountsBatchQueryHandler(_accountContext.Object, _applicationSettings.Object);
        }

        [Test]
        public async Task Handle_ReturnsAccountIds_WhenSuccessful()
        {

            var batchSize = 5;
            var expectedAccountIds = new List<long> { 1, 2, 3, 4, 5 };
            var query = new GetEmailNudgeAccountsBatchQuery { BatchSize = batchSize };

            _accountContext.Setup(x => x.GetEmailNudgeAccountsBatchAsync(batchSize, 30, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAccountIds);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.AccountIds.Should().BeEquivalentTo(expectedAccountIds);
            _accountContext.Verify(x => x.GetEmailNudgeAccountsBatchAsync(batchSize, 30, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}