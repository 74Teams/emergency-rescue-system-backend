using AutoMapper;
using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Common;
using RescueSystem.Application.DTOs.Request;

namespace RescueSystem.Application.Features.Request.Queries.GetAllRequests
{
    public class GetAllRequestsHandler : IRequestHandler<GetAllRequestsQuery, PagedResult<RequestDTO>>
    {
        private readonly IRequestRespository _requestRepository;
        private readonly IMapper _mapper;

        public GetAllRequestsHandler(IRequestRespository requestRepository, IMapper mapper)
        {
            _requestRepository = requestRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<RequestDTO>> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken)
        {
            var result = await _requestRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.Status,
                request.Priority,
                request.EmergencyType,
                request.SortBy);

            var mappedItems = _mapper.Map<List<RequestDTO>>(result.Items);

            return new PagedResult<RequestDTO>
            {
                Items = mappedItems,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize
            };
        }
    }
}
