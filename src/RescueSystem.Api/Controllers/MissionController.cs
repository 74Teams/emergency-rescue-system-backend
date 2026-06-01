using MediatR;
using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Common.Response;
using RescueSystem.Application.DTOs.Mission;
using RescueSystem.Application.Features.Missions.Commands.CreateMission;
using RescueSystem.Application.Features.Missions.Queries.GetMissionById;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using RescueSystem.Application.Features.Missions.Commands.UpdateMission;
using RescueSystem.Application.Features.Missions.Commands.FinishMission;
using RescueSystem.Application.Features.Missions.Queries.GetMissionsPagination;
using RescueSystem.Domain.Entities;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.Features.Missions.Commands.AbortMission;
using RescueSystem.Application.Features.Missions.Queries.GetMissionHistory;
using Microsoft.Identity.Client;

namespace RescueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/missions")]
    public class MissionController(IMediator mediator) : ControllerBase
    {
        // POST api/missions
        [HttpPost]
        // [Authorize(Roles = "Dispatcher")]
        [SwaggerOperation(
        Summary = "Create a new rescue mission",
        Description = "Dispatcher phân công một đội cứu hộ cho một yêu cầu cứu hộ cụ thể"
        )]
        [SwaggerResponse(201, "Mission created successfully", typeof(ApiResponse<Guid>))]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<object>> CreateMission([FromBody] CreateMissionCommand command)
        {

            var res = await mediator.Send(command);

            return StatusCode(201,
                ApiResponse<object>.SuccessResponse(
                    data: new { Id = res },
                    message: "Create mission successfully",
                    statusCode: StatusCodes.Status201Created
                )
            );
        }

        // GET api/missions/{id}
        [HttpGet("{id}")]
        // [Authorize(Roles = "Dispatcher,Rescuer")]
        [SwaggerOperation(
            Summary = "Get mission details by ID",
            Description = "Lấy thông tin chi tiết của 1 nv bằng ID"
            )]
        [SwaggerResponse(200, "Mission details retrieved successfully", typeof(ApiResponse<MissionDTO>))]
        [SwaggerResponse(404, "Mission not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<object>> GetMissionById([FromRoute] Guid id)
        {
            var res = await mediator.Send(new GetMissionByIdQuery { Id = id });
            return Ok(
                ApiResponse<MissionDetailDTO>.SuccessResponse(
                    data: res,
                    message: "Get mission details successfully",
                    statusCode: StatusCodes.Status200OK
                )
            );
        }

        // GET api/missions

        [HttpGet]
        // [Authorize(Roles = "Dispatcher")]
        [SwaggerOperation(
            Summary = "Get missions with pagination",
            Description = "Lấy danh sách nhiệm vụ có phân trang và lọc"
        )]
        [SwaggerResponse(200, "Get missions successfully")]
        public async Task<ActionResult<object>> GetMissions([FromQuery] GetMissionsPaginationQuery query)
        {
            var res = await mediator.Send(query);

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    data: res,
                    message: "Get missions successfully",
                    statusCode: StatusCodes.Status200OK
                )
            );
        }



        // PUT api/missions/{id}/status
        // [Authorize(Roles = "Rescuer,Dispatcher")]
        [HttpPut("{id}/status")]
        [SwaggerOperation(
            Summary = "Update mission status",
            Description = "Cập nhật trạng thái nhiệm vụ"
        )]
        public async Task<ActionResult<object>> UpdateMissionStatus(Guid id, UpdateMissionCommand command)
        {
            command.MissionId = id;

            var res = await mediator.Send(command);

            if (!res)
            {
                throw new BadRequestException("Không thể cập nhật trạng thái nhiệm vụ");
            }

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    data: null,
                    message: "Update mission successfully",
                    statusCode: StatusCodes.Status200OK
                )
            );
        }

        // PUT api/missions/{id}/finish
        //Note: Nên chuyển thành RescuerLeader
        //EDIT: DIEU 17/05/2026: Thêm 2 thuộc tính là userId và note
        private Guid? GetCurrentUserId()
        {
            var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(id, out var guid) ? guid : null;
        }

        [Authorize(Roles = "Rescuer,Dispatcher")]
        [HttpPut("{id}/finish")]
        [SwaggerOperation(
            Summary = "Finish mission",
            Description = "Đánh dấu nhiệm vụ hoàn thành"
        )]
        public async Task<ActionResult<object>> FinishMission(Guid id)
        {
            var command = new FinishMissionCommand
            {
                MissionId = id,
                ChangedById = GetCurrentUserId() ?? Guid.Empty,
                Note = "Hoàn thành nhiệm vụ"
            };

            var res = await mediator.Send(command);

            if (!res)
            {
                throw new BadRequestException("Không thể hoàn thành nhiệm vụ");
            }

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    data: null,
                    message: "Finish mission successfully",
                    statusCode: StatusCodes.Status200OK
                )
            );
        }

        // PUT api/missions/{id}/abort
        // [Authorize(Roles = "Rescuer,Dispatcher")]
        [HttpPut("{id}/abort")]
        [SwaggerOperation(
            Summary = "Abort mission",
            Description = "Hủy bỏ nhiệm vụ"
        )]
        public async Task<ActionResult<object>> AbortMission(Guid id)
        {
            var command = new AbortMissionCommand
            {
                MissionId = id
            };

            var res = await mediator.Send(command);

            if (!res)
            {
                throw new BadRequestException("Không thể hủy bỏ nhiệm vụ");
            }

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    data: null,
                    message: "Abort mission successfully",
                    statusCode: StatusCodes.Status200OK
                )
            );
        }
        // GET api/missions/{id}/history
        [HttpGet("{id}/history")]
        // [Authorize]
        [SwaggerOperation(
            Summary = "Get mission history timeline",
            Description = "Lấy lịch sử thay đổi trạng thái của nhiệm vụ (Dòng thời gian)"
        )]
        [SwaggerResponse(200, "Mission history retrieved successfully", typeof(ApiResponse<IEnumerable<MissionHistoryDTO>>))]
        [SwaggerResponse(404, "Mission not found")]
        public async Task<ActionResult<object>> GetMissionHistory([FromRoute] Guid id)
        {
            var res = await mediator.Send(new GetMissionHistoryQuery { MissionId = id });

            return Ok(
                ApiResponse<IEnumerable<MissionHistoryDTO>>.SuccessResponse(
                    data: res,
                    message: "Lấy lịch sử nhiệm vụ thành công",
                    statusCode: StatusCodes.Status200OK
                )
            );
        }

        // POST api/missions/{id}/history
        [HttpPost("{id}/history")]
        [SwaggerOperation(
            Summary = "Add a note to mission history",
            Description = "Thêm một ghi chú hoặc thông báo vào lịch sử nhiệm vụ mà không đổi trạng thái"
        )]
        public async Task<ActionResult<object>> AddMissionHistory(Guid id, [FromBody] RescueSystem.Application.Features.Missions.Commands.AddMissionHistory.AddMissionHistoryCommand command)
        {
            command.MissionId = id;
            if (command.ChangedById == Guid.Empty)
            {
                command.ChangedById = GetCurrentUserId() ?? Guid.Empty;
            }

            var res = await mediator.Send(command);

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    data: null,
                    message: "Add mission history successfully",
                    statusCode: StatusCodes.Status200OK
                )
            );
        }


    }
}


