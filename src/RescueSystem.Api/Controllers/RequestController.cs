using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Common.Response;
using RescueSystem.Application.DTOs.Request;
using RescueSystem.Application.Features.Request.Commands.ChangeRequestStatus;
using RescueSystem.Application.Features.Request.Commands.CreateRequest;
using RescueSystem.Application.Features.Request.Commands.DeleteRequest;
using RescueSystem.Application.Features.Request.Commands.UpdateRequest;
using RescueSystem.Application.Features.Request.Queries.GetAllRequests;
using RescueSystem.Application.Features.Request.Queries.GetRequestById;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RescueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/requests")]
    [Produces("application/json")] 
    public class RequestController(IMediator mediator) : ControllerBase
    {
        // =========================================================================
        // 1. TRUY VẤN DỮ LIỆU (QUERIES)
        // =========================================================================

        [HttpGet]
        [SwaggerOperation(Summary = "Get rescue requests with pagination & filtering", Description = "Lấy danh sách phân trang và lọc các yêu cầu cứu hộ trong hệ thống")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công", typeof(ApiResponse<object>))]
        public async Task<IActionResult> GetRequests([FromQuery] GetAllRequestsQuery query)
        {
            var result = await mediator.Send(query);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Lấy danh sách yêu cầu cứu hộ thành công", StatusCodes.Status200OK));
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get a rescue request by ID", Description = "Lấy thông tin chi tiết của một yêu cầu cứu hộ cụ thể theo Id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công", typeof(ApiResponse<RequestDTO>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy yêu cầu cứu hộ")]
        public async Task<IActionResult> GetRequestById([FromRoute] Guid id)
        {
            var query = new GetRequestByIdQuery { RequestId = id };
            var result = await mediator.Send(query);
            return Ok(ApiResponse<RequestDTO>.SuccessResponse(result, "Lấy yêu cầu cứu hộ thành công", StatusCodes.Status200OK));
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new rescue request", Description = "Tạo một yêu cầu xin cứu hộ khẩn cấp lên hệ thống")]
        [SwaggerResponse(StatusCodes.Status201Created, "Khởi tạo thành công", typeof(ApiResponse<object>))]
        public async Task<IActionResult> CreateRequest([FromForm] CreateRequestCommand command)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                command.UserId = Guid.Parse(userIdClaim);
            }
            
            var result = await mediator.Send(command);
            var responseData = new { Id = result.Id };
            
            return StatusCode(StatusCodes.Status201Created, ApiResponse<object>.SuccessResponse(responseData, "Tạo yêu cầu cứu hộ thành công", StatusCodes.Status201Created));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Dispatcher,Commander")]
        [SwaggerOperation(Summary = "Update a rescue request", Description = "Cập nhật chỉnh sửa các nội dung chi tiết của một yêu cầu cứu hộ")]
        [SwaggerResponse(StatusCodes.Status200OK, "Cập nhật thành công", typeof(ApiResponse<object>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy yêu cầu cứu hộ")]
        public async Task<IActionResult> UpdateRequest([FromRoute] Guid id, [FromForm] UpdateRequestCommand command)
        {
            command.RequestId = id;
            var result = await mediator.Send(command);
            var responseData = new { Id = result.Id };

            return Ok(ApiResponse<object>.SuccessResponse(responseData, "Cập nhật yêu cầu cứu hộ thành công", StatusCodes.Status200OK));
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Dispatcher,Commander")]
        [SwaggerOperation(Summary = "Change status of a rescue request", Description = "Thay đổi trạng thái xử lý của yêu cầu cứu hộ")]
        [SwaggerResponse(StatusCodes.Status200OK, "Cập nhật trạng thái thành công", typeof(ApiResponse<object>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy yêu cầu cứu hộ")]
        public async Task<IActionResult> ChangeRequestStatus([FromRoute] Guid id, [FromBody] ChangeRequestStatusCommand command)
        {
            command.RequestId = id;
            await mediator.Send(command);
            
            return Ok(ApiResponse<object>.SuccessResponse(null, "Cập nhật trạng thái yêu cầu cứu hộ thành công", StatusCodes.Status200OK));
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Delete a rescue request", Description = "Xóa một yêu cầu cứu hộ khỏi danh sách")]
        [SwaggerResponse(StatusCodes.Status200OK, "Xóa thành công", typeof(ApiResponse<object>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy yêu cầu cứu hộ")]
        public async Task<IActionResult> DeleteRequest([FromRoute] Guid id)
        {
            var command = new DeleteRequestCommand { RequestId = id };
            var result = await mediator.Send(command);
            var responseData = new { Deleted = result };

            return Ok(ApiResponse<object>.SuccessResponse(responseData, "Xóa yêu cầu cứu hộ thành công", StatusCodes.Status200OK));
        }
    }
}