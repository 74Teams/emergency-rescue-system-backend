using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Common.Response;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Application.DTOs.Mission;
using RescueSystem.Application.Features.RescueTeam.Commands.CreateRescueTeam;
using RescueSystem.Application.Features.RescueTeam.Commands.AddMemberToRescueTeam;
using RescueSystem.Application.Features.RescueTeam.Commands.RemoveMemberFromRescueTeam;
using RescueSystem.Application.Features.RescueTeam.Queries.GetAllRescueTeams;
using RescueSystem.Application.Features.RescueTeam.Queries.GetMembersByTeamId;
using RescueSystem.Application.Features.RescueTeam.Queries.GetRescueTeamById;
using RescueSystem.Application.Features.RescueTeam.Commands.UpdateTeamStatus;
using RescueSystem.Application.Features.RescueTeam.Commands.DeleteRescueTeam;
using RescueSystem.Application.Features.RescueTeam.Queries.GetMissionsByTeamId;
using RescueSystem.Application.Features.RescueTeam.Commands.AssignTeamLeader;
using RescueSystem.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RescueSystem.Api.Controllers
{
    [ApiController]
    // Hợp nhất đường dẫn tường minh, tránh việc đổi tên Class làm gãy Route của Frontend
    [Route("api/RescueTeam")] 
    [Produces("application/json")] // Ép toàn bộ API trả về định dạng JSON chuẩn hóa
    public class RescueTeamController(IMediator mediator) : ControllerBase
    {
        // =========================================================================
        // 1. TRUY VẤN DỮ LIỆU (QUERIES)
        // =========================================================================

        [HttpGet]
        [SwaggerOperation(Summary = "Get all rescue teams", Description = "Lấy tất cả danh sách các đội cứu hộ trong hệ thống")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công", typeof(ApiResponse<IEnumerable<RescueTeamDTO>>))]
        public async Task<IActionResult> GetAllRescueTeams()
        {
            var result = await mediator.Send(new GetAllRescueTeamsQuery());
            return Ok(ApiResponse<IEnumerable<RescueTeamDTO>>.SuccessResponse(result, "Get all rescue teams successfully", StatusCodes.Status200OK));
        }

        [HttpGet("{teamId:guid}")] // Ràng buộc định dạng GUID ngay tại Route để chặn request rác
        [SwaggerOperation(Summary = "Get rescue team by id", Description = "Lấy thông tin chi tiết một đội cứu hộ theo mã định danh Id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công", typeof(ApiResponse<RescueTeamDTO>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy đội cứu hộ")]
        public async Task<IActionResult> GetRescueTeamById([FromRoute] Guid teamId)
        {
            var result = await mediator.Send(new GetRescueTeamByIdQuery { Id = teamId });
            return Ok(ApiResponse<RescueTeamDTO>.SuccessResponse(result, "Get rescue team successfully", StatusCodes.Status200OK));
        }

        [HttpGet("{teamId:guid}/members")]
        [SwaggerOperation(Summary = "Get members of a rescue team", Description = "Lấy danh sách tất cả các thành viên hiện đang trực thuộc đội cứu hộ")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công", typeof(ApiResponse<IEnumerable<RescueTeamMemberDTO>>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy đội cứu hộ")]
        public async Task<IActionResult> GetMembersByTeamId([FromRoute] Guid teamId)
        {
            var result = await mediator.Send(new GetMembersByTeamIdQuery { TeamId = teamId });
            return Ok(ApiResponse<IEnumerable<RescueTeamMemberDTO>>.SuccessResponse(result, "Get members of rescue team successfully", StatusCodes.Status200OK));
        }

        [HttpGet("{teamId:guid}/missions")]
        [SwaggerOperation(Summary = "Get missions of a rescue team", Description = "Lấy toàn bộ danh sách các lịch sử nhiệm vụ mà đội cứu hộ này đã hoặc đang tham gia")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công", typeof(ApiResponse<List<MissionDTO>>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy đội cứu hộ")]
        public async Task<IActionResult> GetMissionsByTeamId([FromRoute] Guid teamId)
        {
            var result = await mediator.Send(new GetMissionsByTeamIdQuery { TeamId = teamId });
            return Ok(ApiResponse<List<MissionDTO>>.SuccessResponse(result, "Get missions of rescue team successfully", StatusCodes.Status200OK));
        }

        // =========================================================================
        // 2. THAO TÁC NGHIỆP VỤ (COMMANDS)
        // =========================================================================

        [HttpPost]
        [SwaggerOperation(Summary = "Create a Rescue Team", Description = "Khởi tạo một đội cứu hộ tác chiến mới trên hệ thống")]
        [SwaggerResponse(StatusCodes.Status201Created, "Khởi tạo thành công", typeof(ApiResponse<object>))]
        public async Task<IActionResult> CreateRescueTeam([FromBody] CreateRescueTeamCommand command)
        {
            await mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<object>.SuccessResponse(null, "Rescue team created successfully", StatusCodes.Status201Created));
        }

        [HttpPut("{teamId:guid}/status/{newStatus}")]
        [SwaggerOperation(Summary = "Update rescue team status", Description = "Cập nhật trạng thái hoạt động thực địa của đội cứu hộ (AVAILABLE, ON_MISSION,...)")]
        [SwaggerResponse(StatusCodes.Status200OK, "Cập nhật thành công", typeof(ApiResponse<object>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy đội cứu hộ")]
        public async Task<IActionResult> UpdateRescueTeamStatus([FromRoute] Guid teamId, [FromRoute] TeamStatus newStatus)
        {
            await mediator.Send(new UpdateTeamStatusCommand { TeamId = teamId, NewStatus = newStatus });
            return Ok(ApiResponse<object>.SuccessResponse(null, "Cập nhật trạng thái thành công", StatusCodes.Status200OK));
        }

        [HttpPost("{teamId:guid}/member/{memberId:guid}")]
        [SwaggerOperation(Summary = "Add member to rescue team", Description = "Điều động và thêm một cứu hộ viên vào biên chế đội hình")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thêm thành viên thành công", typeof(ApiResponse<object>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy đội hoặc cứu hộ viên")]
        public async Task<IActionResult> AddMemberToTeam([FromRoute] Guid teamId, [FromRoute] Guid memberId)
        {
            await mediator.Send(new AddMemberToRescueTeamCommand { TeamId = teamId, MemberId = memberId });
            return Ok(ApiResponse<object>.SuccessResponse(null, "Thêm thành viên vào đội thành công", StatusCodes.Status200OK));
        }

        [HttpDelete("{teamId:guid}/member/{memberId:guid}")]
        [SwaggerOperation(Summary = "Remove member from rescue team", Description = "Rút biên chế hoặc điều chuyển cứu hộ viên ra khỏi đội hình")]
        [SwaggerResponse(StatusCodes.Status200OK, "Gỡ bỏ thành viên thành công", typeof(ApiResponse<object>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy thông tin biên chế")]
        public async Task<IActionResult> RemoveMemberFromTeam([FromRoute] Guid teamId, [FromRoute] Guid memberId)
        {
            await mediator.Send(new RemoveMemberFromRescueTeamCommand { TeamId = teamId, MemberId = memberId });
            return Ok(ApiResponse<object>.SuccessResponse(null, "Xóa thành viên khỏi đội thành công", StatusCodes.Status200OK));
        }

        [HttpDelete("{teamId:guid}")]
        [SwaggerOperation(Summary = "Delete a rescue team", Description = "Giải thể hoặc xóa thông tin đội cứu hộ khỏi hệ thống (Khuyên dùng Soft Delete)")]
        [SwaggerResponse(StatusCodes.Status200OK, "Giải thể thành công", typeof(ApiResponse<object>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy đội cứu hộ")]
        public async Task<IActionResult> DeleteRescueTeam([FromRoute] Guid teamId)
        {
            await mediator.Send(new DeleteRescueTeamCommand { Id = teamId });
            return Ok(ApiResponse<object>.SuccessResponse(null, "Rescue team deleted successfully", StatusCodes.Status200OK));
        }
    }
}