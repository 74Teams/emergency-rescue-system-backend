using System.Security.Claims;
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

namespace RescueSystem.Api.Controllers
{
    [Route("api/requests")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private IMediator mediator;
        public RequestController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        // POST api/requests - Create a new rescue request
        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromForm] CreateRequestCommand command)
        {
            var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                command.UserId = Guid.Parse(userIdClaim);
            }
            var result = await mediator.Send(command);
            return StatusCode(201, ApiResponse<object>.SuccessResponse(
                data: new
                {
                    Id = result.Id
                },
                message: "Tạo yêu cầu cứu hộ thành công",
                statusCode: 201
            ));
        }

        // GET api/requests - Get rescue requests with pagination & filtering
        [HttpGet]
        public async Task<IActionResult> GetRequests([FromQuery] GetAllRequestsQuery query)
        {
            var result = await mediator.Send(query);
            return Ok(ApiResponse<object>.SuccessResponse(
                data: result,
                message: "Lấy danh sách yêu cầu cứu hộ thành công",
                statusCode: 200
            ));
        }

        // GET api/requests/{id} - Get a rescue request by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestById(Guid id)
        {
            var query = new GetRequestByIdQuery { RequestId = id };
            var result = await mediator.Send(query);
            return Ok(ApiResponse<RequestDTO>.SuccessResponse(
                data: result,
                message: "Lấy yêu cầu cứu hộ thành công",
                statusCode: 200
            ));
        }

        // PUT api/requests/{id} - Update a rescue request
        [Authorize(Roles = "Dispatcher,Commander")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequest(Guid id, [FromForm] UpdateRequestCommand command)
        {
            command.RequestId = id;
            var result = await mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(
                data: new
                {
                    Id = result.Id
                },
                message: "Cập nhật yêu cầu cứu hộ thành công",
                statusCode: 200
            ));
        }

        // DELETE api/requests/{id} - Delete a rescue request
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(Guid id)
        {
            var command = new DeleteRequestCommand { RequestId = id };
            var result = await mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(
                data: new { Deleted = result },
                message: "Xóa yêu cầu cứu hộ thành công",
                statusCode: 200
            ));
        }

        //Edited by Dieu 9:35 16/05/2026
        //PUT api/requests/{id}/status - Change status of a rescue request
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Dispatcher,Commander")]
        public async Task<IActionResult> ChangeRequestStatus(Guid id, [FromBody] ChangeRequestStatusCommand command)
        {
            command.RequestId = id;
            var result = await mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Cập nhật trạng thái yêu cầu cứu hộ thành công", 200));
        }
    }
}
