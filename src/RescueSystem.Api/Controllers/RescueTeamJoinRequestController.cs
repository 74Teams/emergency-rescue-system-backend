using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Common.Response;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Application.Features.RescueTeamJoinRequest.Commands.CreateJoinRequest;
using RescueSystem.Application.Features.RescueTeamJoinRequest.Commands.ApproveJoinRequest;
using RescueSystem.Application.Features.RescueTeamJoinRequest.Commands.RejectJoinRequest;
using RescueSystem.Application.Features.RescueTeamJoinRequest.Queries.GetRescuerJoinRequestStatus;
using RescueSystem.Application.Features.RescueTeamJoinRequest.Queries.GetTeamJoinRequests;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RescueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/rescueteam/join-requests")]
    [Produces("application/json")]
    [Authorize]
    public class RescueTeamJoinRequestController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Rescuer")]
        [SwaggerOperation(Summary = "Create a rescue team join request", Description = "Gửi yêu cầu xin gia nhập một đội cứu hộ cụ thể.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Tạo yêu cầu thành công", typeof(ApiResponse<RescueTeamJoinRequestDTO>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ hoặc người dùng đã có đội/yêu cầu khác")]
        public async Task<IActionResult> CreateJoinRequest([FromBody] CreateJoinRequestCommand command)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                command.RescuerId = Guid.Parse(userIdClaim);
            }

            var result = await mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<RescueTeamJoinRequestDTO>.SuccessResponse(result, "Gửi đơn yêu cầu gia nhập đội cứu hộ thành công", StatusCodes.Status201Created));
        }

        [HttpGet("my-status")]
        [Authorize(Roles = "Rescuer")]
        [SwaggerOperation(Summary = "Get rescuer's pending join request status", Description = "Lấy thông tin yêu cầu gia nhập hiện tại đang chờ duyệt của cứu hộ viên đang đăng nhập.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công", typeof(ApiResponse<RescueTeamJoinRequestDTO>))]
        public async Task<IActionResult> GetMyStatus()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var result = await mediator.Send(new GetRescuerJoinRequestStatusQuery { RescuerId = Guid.Parse(userIdClaim) });
            return Ok(ApiResponse<RescueTeamJoinRequestDTO?>.SuccessResponse(result, "Lấy trạng thái yêu cầu gia nhập thành công", StatusCodes.Status200OK));
        }

        [HttpGet("pending")]
        [Authorize(Roles = "RescuerLeader,Commander")]
        [SwaggerOperation(Summary = "Get pending join requests for a team or all teams", Description = "Lấy danh sách các yêu cầu gia nhập đang chờ duyệt. Commander xem được tất cả, RescuerLeader xem được của đội mình.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công", typeof(ApiResponse<List<RescueTeamJoinRequestDTO>>))]
        public async Task<IActionResult> GetPendingRequests([FromQuery] Guid? teamId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var query = new GetTeamJoinRequestsQuery { TeamId = teamId, UserId = Guid.Parse(userIdClaim) };
            var result = await mediator.Send(query);
            return Ok(ApiResponse<List<RescueTeamJoinRequestDTO>>.SuccessResponse(result, "Lấy danh sách yêu cầu gia nhập đang chờ duyệt thành công", StatusCodes.Status200OK));
        }

        [HttpPost("{id:guid}/approve")]
        [Authorize(Roles = "RescuerLeader,Commander")]
        [SwaggerOperation(Summary = "Approve a join request", Description = "Đồng ý nhận một cứu hộ viên vào đội cứu hộ.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công", typeof(ApiResponse<object>))]
        public async Task<IActionResult> ApproveRequest([FromRoute] Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var command = new ApproveJoinRequestCommand { RequestId = id, ApprovedById = Guid.Parse(userIdClaim) };
            await mediator.Send(command);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Phê duyệt yêu cầu gia nhập thành công", StatusCodes.Status200OK));
        }

        [HttpPost("{id:guid}/reject")]
        [Authorize(Roles = "RescuerLeader,Commander")]
        [SwaggerOperation(Summary = "Reject a join request", Description = "Từ chối nhận cứu hộ viên vào đội cứu hộ.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công", typeof(ApiResponse<object>))]
        public async Task<IActionResult> RejectRequest([FromRoute] Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var command = new RejectJoinRequestCommand { RequestId = id, RejectedById = Guid.Parse(userIdClaim) };
            await mediator.Send(command);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Từ chối yêu cầu gia nhập thành công", StatusCodes.Status200OK));
        }
    }
}
