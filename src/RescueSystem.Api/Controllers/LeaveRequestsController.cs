using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Common.Response;
using RescueSystem.Application.DTOs.LeaveRequest;
using RescueSystem.Application.Features.LeaveRequest.Commands.ApproveLeaveRequest;
using RescueSystem.Application.Features.LeaveRequest.Commands.CreateLeaveRequest;
using RescueSystem.Application.Features.LeaveRequest.Commands.RejectLeaveRequest;
using RescueSystem.Application.Features.LeaveRequest.Queries.GetMyLeaveRequests;
using RescueSystem.Application.Features.LeaveRequest.Queries.GetTeamLeaveRequests;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RescueSystem.Api.Controllers
{
    [Route("api/leave-requests")]
    [ApiController]
    [Authorize]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeaveRequestsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Rescuer")]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] CreateLeaveRequestCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var rescuerId))
            {
                return Unauthorized();
            }

            command.RescuerId = rescuerId;

            var result = await _mediator.Send(command);
            return Ok(ApiResponse<LeaveRequestDTO>.SuccessResponse(result, "Tạo đơn xin nghỉ phép thành công", StatusCodes.Status200OK));
        }

        [HttpGet("me")]
        [Authorize(Roles = "Rescuer")]
        public async Task<IActionResult> GetMyLeaveRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var rescuerId))
            {
                return Unauthorized();
            }

            var query = new GetMyLeaveRequestsQuery { RescuerId = rescuerId };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<LeaveRequestDTO>>.SuccessResponse(result, "Lấy danh sách đơn xin nghỉ phép thành công", StatusCodes.Status200OK));
        }

        [HttpGet("team/{teamId}")]
        [Authorize(Roles = "RescuerLeader,Commander")]
        public async Task<IActionResult> GetTeamLeaveRequests(Guid teamId)
        {
            var query = new GetTeamLeaveRequestsQuery { TeamId = teamId };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<LeaveRequestDTO>>.SuccessResponse(result, "Lấy danh sách đơn xin nghỉ phép của đội thành công", StatusCodes.Status200OK));
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "RescuerLeader,Commander")]
        public async Task<IActionResult> ApproveLeaveRequest(Guid id, [FromBody] ApproveLeaveRequestCommand command)
        {
            command.LeaveRequestId = id;
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<LeaveRequestDTO>.SuccessResponse(result, "Phê duyệt đơn xin nghỉ phép thành công", StatusCodes.Status200OK));
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "RescuerLeader,Commander")]
        public async Task<IActionResult> RejectLeaveRequest(Guid id, [FromBody] RejectLeaveRequestCommand command)
        {
            command.LeaveRequestId = id;
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<LeaveRequestDTO>.SuccessResponse(result, "Từ chối đơn xin nghỉ phép thành công", StatusCodes.Status200OK));
        }
    }
}
