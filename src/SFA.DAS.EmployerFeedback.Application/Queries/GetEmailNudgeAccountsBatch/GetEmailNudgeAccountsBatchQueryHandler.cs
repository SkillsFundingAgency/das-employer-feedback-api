using MediatR;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerFeedback.Domain.Configuration;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmailNudgeAccountsBatch
{
    public class GetEmailNudgeAccountsBatchQueryHandler : IRequestHandler<GetEmailNudgeAccountsBatchQuery, GetEmailNudgeAccountsBatchQueryResult>
    {
        private readonly IAccountContext _accountContext;
        private readonly ApplicationSettings _applicationSettings;

        public GetEmailNudgeAccountsBatchQueryHandler(IAccountContext accountContext, IOptions<ApplicationSettings> applicationSettings)
        {
            _accountContext = accountContext;
            _applicationSettings = applicationSettings.Value;
        }

        public async Task<GetEmailNudgeAccountsBatchQueryResult> Handle(GetEmailNudgeAccountsBatchQuery request, CancellationToken cancellationToken)
        {
            var accountIds = await _accountContext.GetEmailNudgeAccountsBatchAsync(request.BatchSize, _applicationSettings.EmailNudgeCheckDays, cancellationToken);

            return new GetEmailNudgeAccountsBatchQueryResult
            {
                AccountIds = accountIds
            };
        }
    }
}