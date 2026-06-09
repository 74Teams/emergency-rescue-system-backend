using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Http;
using RescueSystem.Application.DTOs.Request;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Application.Features.Request.Commands.CreateRequest
{
    public class CreateRequestCommand : IRequest<RescueRequest>
    {
        public Guid? UserId { get; set; }
        public EmergencyType EmergencyType { get; set; }
        //EDIT: Dieu 17/05/2026: Them Priority
        public Priority Priority { get; set; } = Priority.MEDIUM;
        public RequestStatus Status { get; set; } = RequestStatus.PENDING;
        public Guid LocationId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<IFormFile>? Files { get; set; }
    }
}
