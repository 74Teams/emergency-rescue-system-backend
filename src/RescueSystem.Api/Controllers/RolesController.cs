using MediatR;
using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Common.Response;
using RescueSystem.Application.Features.Role.Commands.CreateRole;
using RescueSystem.Application.Features.Role.Commands.DeleteRole;
using RescueSystem.Application.Features.Role.Commands.UpdateRole;
using RescueSystem.Application.Features.Role.Queries.GetAllRoles;
using Swashbuckle.AspNetCore.Annotations;

namespace RescueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [SwaggerOperation(
        Summary = "Create a new role",
        Description = "Tạo role mới"
        )]
        [SwaggerResponse(201, "Role created successfully")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<object>> CreateRole([FromBody] CreateRoleCommand command)
        {
            var res = await mediator.Send(command);

            return StatusCode(201,
                ApiResponse<object>.SuccessResponse(
                    null,
                    "Create role successfully",
                    StatusCodes.Status201Created
                )
            );
        }
        // get all roles
        [HttpGet]
        [SwaggerOperation(
        Summary = "Get all roles",
        Description = "Lấy danh sách tất cả role"
         )]
        [SwaggerResponse(200, "Get roles successfully")]
        public async Task<IActionResult> GetAllRoles()
        {
            var res = await mediator.Send(new GetAllRoleQuery());

            return Ok(ApiResponse<object>.SuccessResponse(res, "Get roles successfully"));
        }


        // get role by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var res = await mediator.Send(new GetRoleByIdQuery { Id = id });

            return Ok(ApiResponse<object>.SuccessResponse(res, "Get role successfully"));
        }

        // update role

        [HttpPut("{id}")]
        [SwaggerOperation(
        Summary = "Update role",
        Description = "Cập nhật role"
        )]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleCommand command)
        {
            command.Id = id;

            var res = await mediator.Send(command);

            if (!res)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Update failed"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Update role successfully"));
        }

        // delete role
        [HttpDelete("{id}")]
        [SwaggerOperation(
        Summary = "Delete role",
        Description = "Xóa role theo Id"
        )]
        [SwaggerResponse(200, "Delete role successfully")]
        [SwaggerResponse(404, "Role not found")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var res = await mediator.Send(new DeleteRoleCommand { Id = id });

            if (!res)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Delete failed"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Delete role successfully"));
        }
    }
}
