using MediatR;
using RescueSystem.Application.DTOs.LeaveRequest;
using System;
using System.Collections.Generic;

namespace RescueSystem.Application.Features.LeaveRequest.Queries.GetTeamLeaveRequests
{
    public class GetTeamLeaveRequestsQuery : IRequest<IEnumerable<LeaveRequestDTO>>
    {
        public Guid TeamId { get; set; }
    }
}
