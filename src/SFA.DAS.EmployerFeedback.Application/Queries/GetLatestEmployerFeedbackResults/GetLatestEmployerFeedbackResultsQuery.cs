using MediatR;
using System;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetLatestEmployerFeedbackResults
{
    public class GetLatestEmployerFeedbackResultsQuery : IRequest<GetLatestEmployerFeedbackResultsQueryResult>
    {
        public long AccountId { get; set; }
        public Guid UserRef { get; set; }
    }
}
