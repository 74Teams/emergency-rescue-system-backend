using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Common.Response;
using RescueSystem.Application.Features.Checklist.Commands.CreateChecklist;
using RescueSystem.Application.Features.Checklist.Commands.DeleteChecklist;
using RescueSystem.Application.Features.Checklist.Commands.UpdateChecklist;
using RescueSystem.Application.Features.Checklist.Queries.GetAllChecklists;
using RescueSystem.Application.Features.Checklist.Queries.GetChecklistDetail;
using RescueSystem.Application.Features.ChecklistItem.Commands.CreateChecklistItem;
using RescueSystem.Application.Features.ChecklistItem.Commands.DeleteChecklistItem;
using RescueSystem.Application.Features.ChecklistItem.Commands.UpdateChecklistItem;
using RescueSystem.Application.Features.ChecklistItem.Queries.GetChecklistItemById;
using RescueSystem.Application.Features.ChecklistItem.Queries.GetChecklistItems;

namespace RescueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/checklist")]
    public class ChecklistController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChecklistController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST api/checklist
        [Authorize(Roles = "Rescuer")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateChecklistCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(
                data: new { Id = id },
                message: "Tạo checklist thành công.",
                statusCode: StatusCodes.Status200OK
            ));
        }

        // GET api/checklist

        [Authorize(Roles = "Dispatcher,Rescuer,Commander")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllChecklistsQuery());

            return Ok(ApiResponse<object>.SuccessResponse(
                data: result,
                message: "Checklists đã được truy xuất thành công.",
                statusCode: StatusCodes.Status200OK
            ));
        }

        // GET api/checklist/{id} - join checklist items

        [Authorize(Roles = "Dispatcher,Rescuer,Commander")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(
                new GetChecklistDetailQuery
                {
                    Id = id
                });

            return Ok(ApiResponse<object>.SuccessResponse(
                data: result,
                message: "Chi tiết checklist đã được lấy thành công.",
                statusCode: StatusCodes.Status200OK
            ));
        }

        // POST /api/checklist/{checklistId}/items

        [Authorize(Roles = "Dispatcher,Commander")]
        [HttpPost("{checklistId}/items")]
        public async Task<IActionResult> CreateItem(Guid checklistId, CreateChecklistItemCommand command)
        {
            command.ChecklistId = checklistId;

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(
                data: new { Id = result },
                message: "Tạo thành công checklist item",
                statusCode: StatusCodes.Status200OK
            ));
        }

        //PUT /api/checklist/items/{id}

        [Authorize(Roles = "Dispatcher,Rescuer,Commander")]
        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, UpdateChecklistItemCommand command)
        {
            command.Id = id;

            await _mediator.Send(command);

            return Ok(ApiResponse<object>.SuccessResponse(
                data: null,
                message: "Cập nhật thành công checklist item",
                statusCode: StatusCodes.Status200OK
            ));
        }

        // PUT api/checklist/{id}

        [Authorize(Roles = "Dispatcher,Commander,Rescuer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateChecklistCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return Ok(ApiResponse<object>.SuccessResponse(
                data: null,
                message: "Cập nhật thành công checklist",
                statusCode: StatusCodes.Status200OK
            ));
        }

        // DELETE /api/checklist/items/{id}

        [Authorize(Roles = "Dispatcher,Commander,Rescuer")]
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            await _mediator.Send(
                new DeleteChecklistItemCommand
                {
                    Id = id
                });

            return Ok(ApiResponse<object>.SuccessResponse(
                data: null,
                message: "Xóa thành công checklist item",
                statusCode: StatusCodes.Status200OK
            ));
        }

        // GET /api/checklist/{id}/items

        [Authorize(Roles = "Dispatcher,Rescuer,Commander")]
        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetItems(Guid id)
        {
            var result = await _mediator.Send(
                new GetChecklistItemsQuery
                {
                    ChecklistId = id
                });

            return Ok(ApiResponse<object>.SuccessResponse(
                data: result,
                message: "Checklist items đã được lấy thành công",
                statusCode: StatusCodes.Status200OK
            ));
        }

        // GET /api/checklist/items/{id}

        [Authorize(Roles = "Dispatcher,Rescuer,Admin")]
        [HttpGet("items/{id}")]
        public async Task<IActionResult> GetItemById(Guid id)
        {
            var result = await _mediator.Send(
                new GetChecklistItemByIdQuery
                {
                    Id = id
                });
            return Ok(ApiResponse<object>.SuccessResponse(
                data: result,
                message: "Checklist item đã được lấy thành công",
                statusCode: StatusCodes.Status200OK
            ));
        }

        // DELETE api/checklist/{id}

        [Authorize(Roles = "Dispatcher,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(
                new DeleteChecklistCommand
                {
                    Id = id
                });

            return Ok(ApiResponse<object>.SuccessResponse(
                data: null,
                message: "Xóa thành công checklist",
                statusCode: StatusCodes.Status200OK
            ));
        }
    }
}
