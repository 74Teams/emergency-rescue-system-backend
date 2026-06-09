using MediatR;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.LeaveRequest;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.LeaveRequest.Queries.GetTeamLeaveRequests
{
    public class GetTeamLeaveRequestsQueryHandler : IRequestHandler<GetTeamLeaveRequestsQuery, IEnumerable<LeaveRequestDTO>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public GetTeamLeaveRequestsQueryHandler(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<IEnumerable<LeaveRequestDTO>> Handle(GetTeamLeaveRequestsQuery request, CancellationToken cancellationToken)
        {
            var leaveRequests = await _leaveRequestRepository.GetByTeamIdAsync(request.TeamId);
            return leaveRequests.Select(LeaveRequestDTO.FromEntity);
        }
    }
}
