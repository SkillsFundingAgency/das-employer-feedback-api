using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Api.Controllers;
using SFA.DAS.EmployerFeedback.Application.Commands.SubmitEmployerFeedback;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Application.Queries.GetOverallEmployerFeedbackResult;
using SFA.DAS.EmployerFeedback.Application.Queries.GetYearlyEmployerFeedbackResult;
using SFA.DAS.EmployerFeedback.Domain.Models;

namespace SFA.DAS.EmployerFeedback.Api.UnitTests.Controllers
{
    [TestFixture]
    public class EmployerFeedbackResultControllerTests
    {
        private Mock<IMediator> _mediator;
        private Mock<ILogger<EmployerFeedbackResultController>> _logger;
        private EmployerFeedbackResultController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<EmployerFeedbackResultController>>();
            _controller = new EmployerFeedbackResultController(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task SubmitEmployerFeedback_ReturnsNoContent_WhenSuccessful()
        {
            var request = new SubmitEmployerFeedbackRequest
            {
                UserRef = Guid.NewGuid(),
                Ukprn = 1,
                AccountId = 2,
                ProviderRating = OverallRating.Good,
                FeedbackSource = 1,
                ProviderAttributes = null
            };
            _mediator.Setup(m => m.Send(It.IsAny<SubmitEmployerFeedbackCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SubmitEmployerFeedbackCommandResponse());

            var result = await _controller.SubmitEmployerFeedback(request);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task SubmitEmployerFeedback_ReturnsBadRequest_WhenValidationExceptionThrown()
        {
            var request = new SubmitEmployerFeedbackRequest();
            _mediator.Setup(m => m.Send(It.IsAny<SubmitEmployerFeedbackCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("validation failed"));

            var result = await _controller.SubmitEmployerFeedback(request);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            Assert.AreEqual("validation failed", badRequest.Value);
        }

        [Test]
        public async Task SubmitEmployerFeedback_ReturnsInternalServerError_WhenExceptionThrown()
        {
            var request = new SubmitEmployerFeedbackRequest();
            _mediator.Setup(m => m.Send(It.IsAny<SubmitEmployerFeedbackCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));

            var result = await _controller.SubmitEmployerFeedback(request);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", objectResult.Value);
        }

        [Test]
        public async Task GetOverallEmployerFeedbackResult_ReturnsOk_WhenMediatorReturnsResult()
        {
            var ukprn = 1234L;
            var resultModel = new OverallEmployerFeedbackResult { Ukprn = ukprn, Stars = 5, ReviewCount = 10, TimePeriod = "All", ProviderAttribute = new List<OverallEmployerFeedbackResultProviderAttribute>() };
            _mediator.Setup(m => m.Send(It.Is<GetOverallEmployerFeedbackResultQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOverallEmployerFeedbackResultQueryResult { OverallEmployerFeedback = resultModel });

            var result = await _controller.GetOverallEmployerFeedbackResult(ukprn);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(resultModel, okResult.Value);
            _mediator.Verify(m => m.Send(It.Is<GetOverallEmployerFeedbackResultQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetOverallEmployerFeedbackResult_ReturnsOkWithNull_WhenNoData()
        {
            var ukprn = 1234L;
            _mediator.Setup(m => m.Send(It.Is<GetOverallEmployerFeedbackResultQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOverallEmployerFeedbackResultQueryResult { OverallEmployerFeedback = null });

            var result = await _controller.GetOverallEmployerFeedbackResult(ukprn);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.IsNull(okResult.Value);
            _mediator.Verify(m => m.Send(It.Is<GetOverallEmployerFeedbackResultQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetOverallEmployerFeedbackResult_ReturnsInternalServerError_WhenExceptionThrown()
        {
            var ukprn = 1234L;
            _mediator.Setup(m => m.Send(It.IsAny<GetOverallEmployerFeedbackResultQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));

            var result = await _controller.GetOverallEmployerFeedbackResult(ukprn);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", objectResult.Value);
            _mediator.Verify(m => m.Send(It.IsAny<GetOverallEmployerFeedbackResultQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetYearlyEmployerFeedbackResult_ReturnsOk_WhenMediatorReturnsResult()
        {
            var ukprn = 1234L;
            var details = new List<GetYearlyEmployerFeedbackResult> {
                new GetYearlyEmployerFeedbackResult { Ukprn = ukprn, Stars = 5, ReviewCount = 10, TimePeriod = "2023", ProviderAttribute = new List<GetYearlyEmployerFeedbackResultProviderAttribute>() }
            };
            var queryResult = new GetYearlyEmployerFeedbackResultQueryResult { AnnualEmployerFeedbackDetails = details };
            _mediator.Setup(m => m.Send(It.Is<GetYearlyEmployerFeedbackResultQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _controller.GetYearlyEmployerFeedbackResult(ukprn);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(queryResult, okResult.Value);
            _mediator.Verify(m => m.Send(It.Is<GetYearlyEmployerFeedbackResultQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetYearlyEmployerFeedbackResult_ReturnsOkWithEmptyList_WhenNoData()
        {
            var ukprn = 1234L;
            var queryResult = new GetYearlyEmployerFeedbackResultQueryResult { AnnualEmployerFeedbackDetails = new List<GetYearlyEmployerFeedbackResult>() };
            _mediator.Setup(m => m.Send(It.Is<GetYearlyEmployerFeedbackResultQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _controller.GetYearlyEmployerFeedbackResult(ukprn);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.IsInstanceOf<GetYearlyEmployerFeedbackResultQueryResult>(okResult.Value);
            Assert.IsEmpty(((GetYearlyEmployerFeedbackResultQueryResult)okResult.Value).AnnualEmployerFeedbackDetails);
            _mediator.Verify(m => m.Send(It.Is<GetYearlyEmployerFeedbackResultQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetYearlyEmployerFeedbackResult_ReturnsBadRequest_WhenValidationExceptionThrown()
        {
            var ukprn = 1234L;
            _mediator.Setup(m => m.Send(It.IsAny<GetYearlyEmployerFeedbackResultQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("validation failed"));

            var result = await _controller.GetYearlyEmployerFeedbackResult(ukprn);
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            Assert.AreEqual("validation failed", badRequest.Value);
            _mediator.Verify(m => m.Send(It.IsAny<GetYearlyEmployerFeedbackResultQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetYearlyEmployerFeedbackResult_ReturnsInternalServerError_WhenExceptionThrown()
        {
            var ukprn = 1234L;
            _mediator.Setup(m => m.Send(It.IsAny<GetYearlyEmployerFeedbackResultQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));

            var result = await _controller.GetYearlyEmployerFeedbackResult(ukprn);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", objectResult.Value);
            _mediator.Verify(m => m.Send(It.IsAny<GetYearlyEmployerFeedbackResultQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetEmployerFeedbackResultForAcademicYear_ReturnsOk_WhenMediatorReturnsResult()
        {
            var ukprn = 1234L;
            var year = "AY2023";
            var feedback = new EmployerFeedbackForAcademicYearResult
            {
                Ukprn = ukprn,
                Stars = 5,
                ReviewCount = 10,
                TimePeriod = year,
                ProviderAttribute = new List<ProviderAttributeForAcademicYearResult>()
            };
            var queryResult = new Application.Queries.GetEmployerFeedbackResultForAcademicYear.GetEmployerFeedbackResultForAcademicYearQueryResult { EmployerFeedbackForAcademicYear = feedback };
            _mediator.Setup(m => m.Send(It.Is<Application.Queries.GetEmployerFeedbackResultForAcademicYear.GetEmployerFeedbackResultForAcademicYearQuery>(q => q.Ukprn == ukprn && q.TimePeriod == year), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _controller.GetEmployerFeedbackResultForAcademicYear(ukprn, year);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(feedback, okResult.Value);
            _mediator.Verify(m => m.Send(It.Is<Application.Queries.GetEmployerFeedbackResultForAcademicYear.GetEmployerFeedbackResultForAcademicYearQuery>(q => q.Ukprn == ukprn && q.TimePeriod == year), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetEmployerFeedbackResultForAcademicYear_ReturnsOkWithEmptyAttributes_WhenNoData()
        {
            var ukprn = 1234L;
            var year = "AY2023";
            var feedback = new EmployerFeedbackForAcademicYearResult
            {
                Ukprn = ukprn,
                Stars = 0,
                ReviewCount = 0,
                TimePeriod = year,
                ProviderAttribute = new List<ProviderAttributeForAcademicYearResult>()
            };
            var queryResult = new Application.Queries.GetEmployerFeedbackResultForAcademicYear.GetEmployerFeedbackResultForAcademicYearQueryResult { EmployerFeedbackForAcademicYear = feedback };
            _mediator.Setup(m => m.Send(It.Is<Application.Queries.GetEmployerFeedbackResultForAcademicYear.GetEmployerFeedbackResultForAcademicYearQuery>(q => q.Ukprn == ukprn && q.TimePeriod == year), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _controller.GetEmployerFeedbackResultForAcademicYear(ukprn, year);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.IsInstanceOf<EmployerFeedbackForAcademicYearResult>(okResult.Value);
            Assert.IsEmpty(((EmployerFeedbackForAcademicYearResult)okResult.Value).ProviderAttribute);
            _mediator.Verify(m => m.Send(It.Is<Application.Queries.GetEmployerFeedbackResultForAcademicYear.GetEmployerFeedbackResultForAcademicYearQuery>(q => q.Ukprn == ukprn && q.TimePeriod == year), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetEmployerFeedbackResultForAcademicYear_ReturnsBadRequest_WhenValidationExceptionThrown()
        {
            var ukprn = 1234L;
            var year = "AY2023";
            _mediator.Setup(m => m.Send(It.IsAny<Application.Queries.GetEmployerFeedbackResultForAcademicYear.GetEmployerFeedbackResultForAcademicYearQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("validation failed"));

            var result = await _controller.GetEmployerFeedbackResultForAcademicYear(ukprn, year);
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            Assert.AreEqual("validation failed", badRequest.Value);
            _mediator.Verify(m => m.Send(It.IsAny<Application.Queries.GetEmployerFeedbackResultForAcademicYear.GetEmployerFeedbackResultForAcademicYearQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetEmployerFeedbackResultForAcademicYear_ReturnsInternalServerError_WhenExceptionThrown()
        {
            var ukprn = 1234L;
            var year = "AY2023";
            _mediator.Setup(m => m.Send(It.IsAny<Application.Queries.GetEmployerFeedbackResultForAcademicYear.GetEmployerFeedbackResultForAcademicYearQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));

            var result = await _controller.GetEmployerFeedbackResultForAcademicYear(ukprn, year);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", objectResult.Value);
            _mediator.Verify(m => m.Send(It.IsAny<Application.Queries.GetEmployerFeedbackResultForAcademicYear.GetEmployerFeedbackResultForAcademicYearQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetEmployerFeedbackRatingsResult_ReturnsOk_WhenMediatorReturnsResult()
        {
            var timePeriod = "AY2023";
            var ratings = new List<EmployerFeedbackRatingsResult>
            {
                new EmployerFeedbackRatingsResult { Ukprn = 12345678, Stars = 5, ReviewCount = 10 }
            };
            var queryResult = new Application.Queries.GetEmployerFeedbackRatingsResult.GetEmployerFeedbackRatingsResultQueryResult { EmployerFeedbackRatings = ratings };
            _mediator.Setup(m => m.Send(It.Is<Application.Queries.GetEmployerFeedbackRatingsResult.GetEmployerFeedbackRatingsResultQuery>(q => q.TimePeriod == timePeriod), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _controller.GetEmployerFeedbackRatingsResult(timePeriod);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(ratings, okResult.Value);
            _mediator.Verify(m => m.Send(It.Is<Application.Queries.GetEmployerFeedbackRatingsResult.GetEmployerFeedbackRatingsResultQuery>(q => q.TimePeriod == timePeriod), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetEmployerFeedbackRatingsResult_ReturnsOkWithEmptyList_WhenNoData()
        {
            var timePeriod = "AY2023";
            var queryResult = new Application.Queries.GetEmployerFeedbackRatingsResult.GetEmployerFeedbackRatingsResultQueryResult { EmployerFeedbackRatings = new List<SFA.DAS.EmployerFeedback.Domain.Models.EmployerFeedbackRatingsResult>() };
            _mediator.Setup(m => m.Send(It.Is<Application.Queries.GetEmployerFeedbackRatingsResult.GetEmployerFeedbackRatingsResultQuery>(q => q.TimePeriod == timePeriod), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _controller.GetEmployerFeedbackRatingsResult(timePeriod);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.IsInstanceOf<List<EmployerFeedbackRatingsResult>>(okResult.Value);
            Assert.IsEmpty((List<EmployerFeedbackRatingsResult>)okResult.Value);
            _mediator.Verify(m => m.Send(It.Is<Application.Queries.GetEmployerFeedbackRatingsResult.GetEmployerFeedbackRatingsResultQuery>(q => q.TimePeriod == timePeriod), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetEmployerFeedbackRatingsResult_ReturnsBadRequest_WhenValidationExceptionThrown()
        {
            var timePeriod = "AY2023";
            _mediator.Setup(m => m.Send(It.IsAny<Application.Queries.GetEmployerFeedbackRatingsResult.GetEmployerFeedbackRatingsResultQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("validation failed"));

            var result = await _controller.GetEmployerFeedbackRatingsResult(timePeriod);
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            Assert.AreEqual("validation failed", badRequest.Value);
            _mediator.Verify(m => m.Send(It.IsAny<Application.Queries.GetEmployerFeedbackRatingsResult.GetEmployerFeedbackRatingsResultQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetEmployerFeedbackRatingsResult_ReturnsInternalServerError_WhenExceptionThrown()
        {
            var timePeriod = "AY2023";
            _mediator.Setup(m => m.Send(It.IsAny<Application.Queries.GetEmployerFeedbackRatingsResult.GetEmployerFeedbackRatingsResultQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));

            var result = await _controller.GetEmployerFeedbackRatingsResult(timePeriod);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", objectResult.Value);
            _mediator.Verify(m => m.Send(It.IsAny<Application.Queries.GetEmployerFeedbackRatingsResult.GetEmployerFeedbackRatingsResultQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
