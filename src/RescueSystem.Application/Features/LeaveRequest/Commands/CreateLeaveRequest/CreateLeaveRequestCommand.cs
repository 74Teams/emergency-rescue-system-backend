using MediatR;
using RescueSystem.Application.DTOs.LeaveRequest;
using System;

namespace RescueSystem.Application.Features.LeaveRequest.Commands.CreateLeaveRequest
{
    public class CreateLeaveRequestCommand : IRequest<LeaveRequestDTO>
    {
        public Guid RescuerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
