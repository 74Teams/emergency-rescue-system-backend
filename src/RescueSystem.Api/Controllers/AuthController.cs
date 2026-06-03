using CloudinaryDotNet.Actions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.Common.Interfaces.Services;
using RescueSystem.Application.Common.Response;
using RescueSystem.Application.DTOs.Auth;
using RescueSystem.Application.Features.Auth.Commands.ForgotPassword;
using RescueSystem.Application.Features.Auth.Commands.Login;
using RescueSystem.Application.Features.Auth.Commands.RefreshToken;
using RescueSystem.Application.Features.Auth.Commands.Register;
using RescueSystem.Application.Features.Auth.Commands.SelectRole;
using RescueSystem.Application.Features.Auth.Commands.ResetPassword;
using RescueSystem.Application.Features.Auth.Commands.UpdateAvatar;
using RescueSystem.Application.Features.Auth.Commands.UpdateProfile;
using RescueSystem.Application.Features.Auth.Queries.Profile;
using RescueSystem.Application.Features.Contact.Commands.CreateContact;
using RescueSystem.Application.Features.Contact.Commands.DeleteContact;
using RescueSystem.Application.Features.Contact.Commands.UpdateContact;
using RescueSystem.Application.Features.Contact.Queries.GetAllContact;
using RescueSystem.Application.Features.Contact.Queries.GetContactById;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RescueSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IMediator _mediator;
        private readonly IEmailService _emailService;
        public AuthController(IMediator mediator, IEmailService emailService)
        {
            _mediator = mediator;
            _emailService = emailService;
        }

        // Post api/auth/register
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Đăng ký tài khoản")]
        public async Task<ActionResult<object>> Register([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(201, ApiResponse<AuthResponse>.SuccessResponse(result, "Đăng ký tài khoản thành công", StatusCodes.Status201Created));
        }

        // Post api/auth/select-role
        [Authorize]
        [HttpPost("select-role")]
        [SwaggerOperation(Summary = "Lựa chọn vai trò cho tài khoản")]
        public async Task<ActionResult<object>> SelectRole([FromBody] SelectRoleCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null)
            {
                throw new UnauthorizedException("Lỗi xác thực người dùng");
            }

            command.UserId = Guid.Parse(userId);
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(result, "Lựa chọn vai trò thành công", StatusCodes.Status200OK));
        }

        // Post api/auth/login
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Đăng nhập tài khoản")]
        public async Task<ActionResult<object>> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(result, "Đăng nhập thành công", StatusCodes.Status200OK));
        }

        // Get api/auth/profile
        [Authorize]
        [HttpGet("profile")]
        [SwaggerOperation(Summary = "Lấy thông tin người dùng")]
        public async Task<ActionResult<object>> Profile()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null)
            {
                throw new UnauthorizedException("Lỗi xác thực người dùng");
            }

            var query = new ProfileQuery
            {
                UserId = userId,
            };
            var response = await _mediator.Send(query);
            return Ok(ApiResponse<ProfileResponse>.SuccessResponse(response, "Lấy thông tin người dùng thành công", StatusCodes.Status200OK));
        }

        // Post api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<object>.SuccessResponse(null, result, StatusCodes.Status200OK));
        }

        // Post api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<object>.SuccessResponse(null, result, StatusCodes.Status200OK));
        }

        // Put api/auth/profile
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null)
            {
                throw new UnauthorizedException("Lỗi xác thực người dùng");
            }

            var result = await _mediator.Send(command);
            return Ok(ApiResponse<object>.SuccessResponse(null, result, StatusCodes.Status200OK));
        }

        //  Post api/auth/avatar
        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] UpdateAvatarCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null)
            {
                throw new UnauthorizedException("Lỗi xác thực người dùng");
            }

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(result, "Cập nhật avatar thành công", StatusCodes.Status200OK));
        }

        // Post api/auth/contact/{id}
        [Authorize]
        [HttpPost("contact")]
        public async Task<IActionResult> Create([FromBody] CreateContactCommand command)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("UserId không hợp lệ", StatusCodes.Status401Unauthorized));
            }

            command.UserId = userId;

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(result, "Tao contact thanh cong", StatusCodes.Status200OK));
        }

        // Put api/auth/contact/{id}
        [Authorize]
        [HttpPut("contact/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactCommand command)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("UserId không hợp lệ", StatusCodes.Status401Unauthorized));

            command.Id = id;
            command.UserId = userId;

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(result, "Cap nhat thong tin thanh cong", StatusCodes.Status200OK));
        }

        // Delete api/auth/contact/{id}
        [Authorize]
        [HttpDelete("contact/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("UserId không hợp lệ", StatusCodes.Status401Unauthorized));

            var command = new DeleteContactCommand
            {
                Id = id,
                UserId = userId
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(new { Deleted = result }, "Xoa thanh cong", StatusCodes.Status200OK));
        }

        // Get api/auth/contact/{id}
        [Authorize]
        [HttpGet("contact/{id}")]
        public async Task<IActionResult> GetContactId(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("UserId không hợp lệ", StatusCodes.Status401Unauthorized));

            var query = new GetContactDetailQuery
            {
                Id = id,
                UserId = userId
            };

            var result = await _mediator.Send(query);

            return Ok(ApiResponse<object>.SuccessResponse(result, "Lay thong tin thanh cong", StatusCodes.Status200OK));
        }

        // Get api/auth/contact
        [Authorize]
        [HttpGet("contact")]
        public async Task<IActionResult> GetAllContacts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("UserId không hợp lệ", StatusCodes.Status401Unauthorized));

            var query = new GetAllContactQuery { UserId = userId };

            var result = await _mediator.Send(query);

            return Ok(ApiResponse<object>.SuccessResponse(result, "Lay thong tin thanh cong", StatusCodes.Status200OK));
        }

        // Post api/auth/refresh
        [HttpPost("refresh")]
        [SwaggerOperation(Summary = "Làm mới access token")]
        public async Task<ActionResult<object>> Refresh([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(result, "Làm mới token thành công", StatusCodes.Status200OK));
        }

    }
}
