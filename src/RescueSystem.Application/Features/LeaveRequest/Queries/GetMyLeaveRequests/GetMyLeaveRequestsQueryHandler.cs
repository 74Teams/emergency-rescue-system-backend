using MediatR;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.LeaveRequest;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.LeaveRequest.Queries.GetMyLeaveRequests
{
    public class GetMyLeaveRequestsQueryHandler : IRequestHandler<GetMyLeaveRequestsQuery, IEnumerable<LeaveRequestDTO>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public GetMyLeaveRequestsQueryHandler(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<IEnumerable<LeaveRequestDTO>> Handle(GetMyLeaveRequestsQuery request, CancellationToken cancellationToken)
        {
            var leaveRequests = await _leaveRequestRepository.GetByRescuerIdAsync(request.RescuerId);
            return leaveRequests.Select(LeaveRequestDTO.FromEntity);
        }
    }
}
