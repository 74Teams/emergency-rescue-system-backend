using MediatR;
using RescueSystem.Application.DTOs.LeaveRequest;
using System;
using System.Collections.Generic;

namespace RescueSystem.Application.Features.LeaveRequest.Queries.GetMyLeaveRequests
{
    public class GetMyLeaveRequestsQuery : IRequest<IEnumerable<LeaveRequestDTO>>
    {
        public Guid RescuerId { get; set; }
    }
}
