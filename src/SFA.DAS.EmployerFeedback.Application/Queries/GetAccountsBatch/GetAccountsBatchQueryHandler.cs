using MediatR;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerFeedback.Domain.Configuration;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetAccountsBatch
{
    public class GetAccountsBatchQueryHandler : IRequestHandler<GetAccountsBatchQuery, GetAccountsBatchQueryResult>
    {
        private readonly IAccountContext _accountContext;
        private readonly ApplicationSettings _applicationSettings;

        public GetAccountsBatchQueryHandler(IAccountContext accountContext, IOptions<ApplicationSettings> applicationSettings)
        {
            _accountContext = accountContext;
            _applicationSettings = applicationSettings.Value;
        }

        public async Task<GetAccountsBatchQueryResult> Handle(GetAccountsBatchQuery request, CancellationToken cancellationToken)
        {
            var accountIds = await _accountContext.GetAccountsBatchAsync(request.BatchSize, _applicationSettings.BatchDays, cancellationToken);

            return new GetAccountsBatchQueryResult
            {
                AccountIds = accountIds
            };
        }
    }
}