using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Common.Response;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Application.Features.RescueTeam.Command.CreateRescueTeam;
using RescueSystem.Application.Features.RescueTeam.Commands.AddMemberToRescueTeam;
using RescueSystem.Application.Features.RescueTeam.Commands.RemoveMemberFromRescueTeam;
using RescueSystem.Application.Features.RescueTeam.Queries.GetAllRescueTeams;
using RescueSystem.Application.Features.RescueTeam.Queries.GetMembersByTeamId;
using RescueSystem.Application.Features.RescueTeam.Queries.GetRescueTeamById;
using RescueSystem.Application.Features.RescueTeam.Commands.UpdateTeamStatus;
using RescueSystem.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;
using RescueSystem.Application.Features.RescueTeam.Commands.DeleteRescueTeam;
using RescueSystem.Application.DTOs.Mission;
using RescueSystem.Application.Features.RescueTeam.Queries.GetMissionsByTeamId;
using Microsoft.AspNetCore.Authorization;

namespace RescueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RescueTeamController(IMediator mediator) : ControllerBase
    {
        // GET: api/RescueTeam
        [HttpGet]
        //[Authorize(Roles = "Dispatcher,RescuerLeader")]
        [SwaggerOperation(
            Summary = "Get all rescue teams",
            Description = "Lấy tất cả các đội cứu hộ"
        )]
        [SwaggerResponse(200, "Success", typeof(ApiResponse<RescueTeamDTO[]>))]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<object>> GetAllRescueTeams()
        {
            var rescueTeams = await mediator.Send(new GetAllRescueTeamsQuery());
            return Ok(ApiResponse<object>.SuccessResponse(rescueTeams, "Get all rescue teams successfully", StatusCodes.Status200OK));
        }
        // GET: api/RescueTeam/{id}
        [HttpGet("{teamId}")]
        //[Authorize(Roles = "Dispatcher,RescuerLeader,Rescuer,Commander")]
        [SwaggerOperation(
            Summary = "Get rescue team by id",
            Description = "Lấy một đội cứu hộ theo Id"
        )]
        [SwaggerResponse(200, "Success", typeof(ApiResponse<RescueTeamDTO>))]
        [SwaggerResponse(404, "Rescue team not found")]
        public async Task<ActionResult<object>> GetRescueTeamById([FromRoute] Guid teamId)
        {
            var team = await mediator.Send(new GetRescueTeamByIdQuery { Id = teamId });
            return Ok(ApiResponse<RescueTeamDTO>.SuccessResponse(team, "Get rescue team successfully", StatusCodes.Status200OK));
        }
        // POST: api/RescueTeam
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a Rescue Team",
            Description = "Tạo đội cứu hộ mới"
        )]
        [SwaggerResponse(201, "User created successfully")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<object>> CreateRescueTeam([FromBody] CreateRescueTeamCommand dto)
        {
            var res = await mediator.Send(dto);

            return StatusCode(StatusCodes.Status201Created, ApiResponse<object>.SuccessResponse(null, "Rescue team created successfully", StatusCodes.Status201Created));
        }
        // PUT: api/RescueTeam/{id}
        [HttpPut("{teamId}/status/{newStatus}")]
        [SwaggerOperation(
            Summary = "Update rescue team status",
            Description = "Cập nhật trạng thái của đội cứu hộ"
        )]
        [SwaggerResponse(200, "Status updated successfully")]
        [SwaggerResponse(404, "Rescue team not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<object>> UpdateRescueTeamStatus([FromRoute] Guid teamId, [FromRoute] TeamStatus newStatus)
        {
            var command = new UpdateTeamStatusCommand
            {
                TeamId = teamId,
                NewStatus = newStatus,
            };

            await mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Cập nhật trạng thái thành công", StatusCodes.Status200OK));
        }
        // DELETE: api/RescueTeam/{id}
        //Note: Thực tế thì dùng delete để xóa cứng là kh ổn, nên dùng soft delete(cập nhật 1 trường deleted thành true) tránh ảnh hưởng đến mấy cái foreign key
        [HttpDelete("{teamId}")]
        [SwaggerOperation(
            Summary = "Delete a rescue team",
            Description = "Xóa một đội cứu hộ"
        )]
        [SwaggerResponse(200, "Rescue team deleted successfully")]
        [SwaggerResponse(404, "Rescue team not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<object>> DeleteRescueTeam([FromRoute] Guid teamId)
        {
            var command = new DeleteRescueTeamCommand { Id = teamId };
            await mediator.Send(command);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Rescue team deleted successfully", StatusCodes.Status200OK));
        }

        // GET : api/rescue-teams/{teamId}/members
        [HttpGet("{teamId}/members")]
        [SwaggerOperation(
            Summary = "Get members of a rescue team",
            Description = "Lấy các thành viên của một đội cứu hộ"
        )]
        [SwaggerResponse(200, "Success", typeof(ApiResponse<RescueTeamMemberDTO[]>))]
        [SwaggerResponse(404, "Rescue team not found")]
        public async Task<ActionResult<object>> GetMembersByTeamId([FromRoute] Guid teamId)
        {
            var members = await mediator.Send(new GetMembersByTeamIdQuery { TeamId = teamId });
            return Ok(ApiResponse<object>.SuccessResponse(members, "Get members of rescue team successfully", StatusCodes.Status200OK));
        }

        // POST : api/rescue-teams/{teamId}/members
        [HttpPost("{teamId}/member/{memberId}")]
        [SwaggerOperation(
            Summary = "Add member to rescue team",
            Description = "Thêm thành viên vào đội cứu hộ"
        )]
        [SwaggerResponse(200, "Member added successfully")]
        [SwaggerResponse(404, "Rescue team or member not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<object>> AddMemberToTeam([FromRoute] Guid teamId, [FromRoute] Guid memberId)
        {

            var command = new AddMemberToRescueTeamCommand
            {
                TeamId = teamId,
                MemberId = memberId
            };

            await mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Thêm thành viên thành công", StatusCodes.Status200OK));
        }

        // DELETE : api/RescueTeam/{teamId}/member/{memberId}
        [HttpDelete("{teamId}/member/{memberId}")]
        [SwaggerOperation(
            Summary = "Remove member from rescue team",
            Description = "Xóa thành viên khỏi đội cứu hộ"
        )]
        [SwaggerResponse(200, "Member removed successfully")]
        [SwaggerResponse(404, "Rescue team or member not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<object>> RemoveMemberFromTeam([FromRoute] Guid teamId, [FromRoute] Guid memberId)
        {
            var command = new RemoveMemberFromRescueTeamCommand
            {
                TeamId = teamId,
                MemberId = memberId
            };

            await mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Xóa thành viên thành công", StatusCodes.Status200OK));
        }
        // GET : api/RescueTeam/{teamId}/missions
        //TODO :CHECK
        [HttpGet("{teamId}/missions")]
        [SwaggerOperation(
            Summary = "Get missions of a rescue team",
            Description = "Lấy toàn bộ nhiệm vụ của một đội cứu hộ"
        )]
        [SwaggerResponse(200, "Success", typeof(ApiResponse<List<MissionDTO>>))]
        [SwaggerResponse(404, "Rescue team not found")]
        public async Task<ActionResult<object>> GetMissionsByTeamId([FromRoute] Guid teamId)
        {
            var missions = await mediator.Send(new GetMissionsByTeamIdQuery { TeamId = teamId });
            return Ok(
                ApiResponse<object>.SuccessResponse(
                    missions,
                    "Get missions of rescue team successfully",
                    StatusCodes.Status200OK
                )
            );
        }
        // POST : api/RescueTeam/{teamId}/missions
        // DELETE : api/RescueTeam/{teamId}/missions/{missionId}
    }
}
