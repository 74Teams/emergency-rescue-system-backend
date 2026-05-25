using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [Authorize(Roles = "Dispatcher,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateChecklistCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Tạo checklist thành công.",
                id = id
            });
        }

        // GET api/checklist

        [Authorize(Roles = "Dispatcher,Rescuer,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllChecklistsQuery());

            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Checklists đã được truy xuất thành công.",
                data = result
            });
        }

        // GET api/checklist/{id} - join checklist items

        [Authorize(Roles = "Dispatcher,Rescuer,Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(
                new GetChecklistDetailQuery
                {
                    Id = id
                });

            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Chi tiết checklist đã được lấy thành công.",
                data = result
            });
        }

        // POST /api/checklist/{checklistId}/items

        [Authorize(Roles = "Dispatcher,Admin")]
        [HttpPost("{checklistId}/items")]
        public async Task<IActionResult> CreateItem(Guid checklistId, CreateChecklistItemCommand command)
        {
            command.ChecklistId = checklistId;

            var result = await _mediator.Send(command);

            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Tạo thành công checklist item",
                id = result
            });
        }

        //PUT /api/checklist/items/{id}

        [Authorize(Roles = "Dispatcher,Rescuer,Admin")]
        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, UpdateChecklistItemCommand command)
        {
            command.Id = id;

            await _mediator.Send(command);

            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Cập nhật thành công checklist item"
            });
        }

        // PUT api/checklist/{id}

        [Authorize(Roles = "Dispatcher,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateChecklistCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Cập nhật thành công checklist"
            });
        }

        // DELETE /api/checklist/items/{id}

        [Authorize(Roles = "Dispatcher,Admin")]
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            await _mediator.Send(
                new DeleteChecklistItemCommand
                {
                    Id = id
                });

            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Xóa thành công checklist item"
            });
        }

        // GET /api/checklist/{id}/items

        [Authorize(Roles = "Dispatcher,Rescuer,Admin")]
        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetItems(Guid id)
        {
            var result = await _mediator.Send(
                new GetChecklistItemsQuery
                {
                    ChecklistId = id
                });

            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Checklist items đã được lấy thành công",
                data = result
            });
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
            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Checklist item đã được lấy thành công",
                data = result
            });
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

            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Xóa thành công checklist"
            });
        }
    }
}
