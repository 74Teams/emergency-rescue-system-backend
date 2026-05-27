using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.DTOs.User;
using RescueSystem.Application.DTOs.Common;
using RescueSystem.Application.DTOs.Commander;
using RescueSystem.Application.Common.Response;
using RescueSystem.Domain.Entities;
using MediatR;
using RescueSystem.Application.Features.User.Commands;
using RescueSystem.Application.Features.User.Queries.GetAllUser;
using RescueSystem.Application.Features.User.Queries.GetUserById;
using RescueSystem.Application.Features.Commander.Commands.ApproveUser;
using RescueSystem.Application.Features.Commander.Commands.RejectUser;
using RescueSystem.Application.Features.Commander.Commands.ToggleUserStatus;
using RescueSystem.Application.Features.Commander.Queries.GetPendingApprovalUsers;
using RescueSystem.Application.Features.Commander.Queries.GetRejectedUsers;
using RescueSystem.Application.Features.User.Queries.GetSystemUsers;
using Swashbuckle.AspNetCore.Annotations;
using RescueSystem.Application.Features.User.Commands.UpdateUser;
using RescueSystem.Application.Features.User.Commands.DeleteUser;
using Microsoft.AspNetCore.Authorization;


namespace RescueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        // POST api/users - Create a new user
        [HttpPost]
        [Authorize(Roles = "Commander")]
        [SwaggerOperation(
            Summary = "Create a new user",
            Description = "Tạo người dùng mới"
        )]
        [SwaggerResponse(201, "User created successfully")]
        [SwaggerResponse(500, "Internal server error")]
        [Authorize(Roles = "Commander")]

        public async Task<ActionResult<object>> CreateUser([FromBody] CreateUserCommand dto)
        {
            var res = await mediator.Send(dto);
            return StatusCode(201, ApiResponse<object>.SuccessResponse(null, "Create user successfully", StatusCodes.Status201Created));
        }

        // GET api/users - Get all users
        [HttpGet]
        [Authorize(Roles = "Commander, Dispatcher")]
        [SwaggerOperation(
            Summary = "Get all users",
            Description = "Lấy thông tin tất cả người dùng"
        )]
        [SwaggerResponse(200, "Success", typeof(ApiResponse<PagedResult<UserDTO>>))]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<object>> GetAllUsers([FromQuery] GetAllUserQuery query)
        {
            var res = await mediator.Send(query);

            return Ok(ApiResponse<object>.SuccessResponse(res, "Get all users successfully", StatusCodes.Status200OK));
        }

        // GET api/users/{id} - Get user by id
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get user by id",
            Description = "Lấy thông tin user theo Id"
        )]
        [SwaggerResponse(200, "Success", typeof(ApiResponse<UserDTO>))]
        [SwaggerResponse(404, "User not found")]
        [SwaggerResponse(500, "Internal server error")]
        [Authorize(Roles = "Dispatcher")]

        public async Task<ActionResult<UserDTO>> GetUserById([FromRoute] Guid id)
        {
            var result = await mediator.Send(new GetUserByIdQuery { Id = id });
            return Ok(ApiResponse<UserDTO>.SuccessResponse(result, "Get user by id successfully", StatusCodes.Status200OK));
        }


        // PUT api/users/{id} - Update user by id
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;

            var result = await mediator.Send(command);

            if (result)
            {
                return Ok(new
                {
                    status = 200,
                    success = true,
                    message = "Cập nhật thông tin người dùng thành công."
                });
            }

            return NotFound(new
            {
                status = 404,
                success = false,
                message = $"Không tìm thấy người dùng có ID: {id}"
            });
        }

        // DELETE api/users/{id} - Delete user by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await mediator.Send(command);

            if (result)
            {
                return Ok(new
                {
                    status = 200,
                    success = true,
                    message = "Đã xóa người dùng thành công."
                });
            }

            return NotFound(new
            {
                status = 404,
                success = false,
                message = "Không tìm thấy người dùng để xóa."
            });

        }

        // Get pending users
        [HttpGet("/api/commander/approvals/pending")]
        public async Task<ActionResult<ApiResponse<List<UserSystemDTO>>>> GetPendingApprovals()
        {
            var query = new GetPendingApprovalUsersQuery();
            var result = await mediator.Send(query);
            if (result.Count == 0)
            {
                return Ok(ApiResponse<List<UserSystemDTO>>.SuccessResponse(null, "Không có tài khoản nào cần phê duyệt", 200));
            }
            return Ok(ApiResponse<List<UserSystemDTO>>.SuccessResponse(result, "Success", StatusCodes.Status200OK));
        }

        // Get rejected users
        [HttpGet("/api/commander/approvals/rejected")]
        public async Task<ActionResult<ApiResponse<List<UserSystemDTO>>>> GetRejected()
        {
            var query = new GetRejectedUsersQuery();
            var result = await mediator.Send(query);
            if (result.Count == 0)
            {
                return Ok(ApiResponse<List<UserSystemDTO>>.SuccessResponse(null, "Không có tài khoản nào bị từ chối", 200));
            }
            return Ok(ApiResponse<List<UserSystemDTO>>.SuccessResponse(result, "Success", StatusCodes.Status200OK));
        }

        // Get all system users
        [HttpGet("/api/commander/users")]
        public async Task<ActionResult<ApiResponse<List<UserSystemDTO>>>> GetSystemUsers([FromQuery] string? search, [FromQuery] string? role)
        {
            var query = new GetSystemUsersQuery { Search = search, Role = role };
            var result = await mediator.Send(query);
            return Ok(ApiResponse<List<UserSystemDTO>>.SuccessResponse(result, "Success", StatusCodes.Status200OK));
        }

        // Approve user
        [HttpPost("/api/commander/approvals/{userId}")]
        public async Task<ActionResult<ApiResponse<object>>> ApproveUser([FromRoute] Guid userId)
        {
            var command = new ApproveUserCommand { UserId = userId };
            await mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Đã phê duyệt tài khoản thành công", 200));
        }

        // Reject user
        [HttpPost("/api/commander/approvals/{userId}/reject")]
        public async Task<ActionResult<ApiResponse<object>>> RejectUser([FromRoute] Guid userId)
        {
            var command = new RejectUserCommand { UserId = userId };
            await mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Đã từ chối tài khoản", 200));
        }

        // Toggle user status
        [HttpPut("/api/commander/users/{userId}/status")]
        public async Task<ActionResult<ApiResponse<object>>> ToggleUserStatus(
            [FromRoute] Guid userId,
            [FromBody] ToggleStatusRequestDto request)
        {
            var command = new ToggleUserStatusCommand
            {
                UserId = userId,
                IsActive = request.IsActive
            };

            await mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Cập nhật trạng thái tài khoản thành công", 200));
        }
    }

    public class ToggleStatusRequestDto
    {
        public bool IsActive { get; set; }
    }
}
