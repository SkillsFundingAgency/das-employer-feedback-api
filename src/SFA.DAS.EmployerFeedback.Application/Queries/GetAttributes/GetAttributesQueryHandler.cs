using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetAttributes
{
    public class GetAttributesQueryHandler : IRequestHandler<GetAttributesQuery, GetAttributesQueryResult>
    {
        private readonly IAttributeContext _attributeEntityContext;

        public GetAttributesQueryHandler(IAttributeContext attributeEntityContext)
        {
            _attributeEntityContext = attributeEntityContext;
        }

        public async Task<GetAttributesQueryResult> Handle(GetAttributesQuery request, CancellationToken cancellationToken)
        {
            var attributes = await _attributeEntityContext.GetAll();

            return new GetAttributesQueryResult
            {
                Attributes = attributes.Select(entity => (Domain.Models.Attributes)entity).ToList()
            };
        }
    }
}
