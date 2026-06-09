using MediatR;
using RescueSystem.Application.DTOs.LeaveRequest;
using System;

namespace RescueSystem.Application.Features.LeaveRequest.Commands.ApproveLeaveRequest
{
    public class ApproveLeaveRequestCommand : IRequest<LeaveRequestDTO>
    {
        public Guid LeaveRequestId { get; set; }
        public string? Note { get; set; }
    }
}
