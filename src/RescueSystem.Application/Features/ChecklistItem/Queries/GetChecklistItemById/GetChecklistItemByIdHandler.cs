using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.Features.Checklist;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.ChecklistItem.Queries.GetChecklistItemById
{
    public class GetChecklistItemByIdHandler : IRequestHandler<GetChecklistItemByIdQuery, ChecklistItemDTO>
    {
        private readonly IChecklistItemRepository _itemRepository;

        public GetChecklistItemByIdHandler(IChecklistItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<ChecklistItemDTO> Handle(GetChecklistItemByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await _itemRepository
                .GetByIdAsync(request.Id);

            if (item == null)
            {
                throw new Exception("Không tìm thấy checklist item");
            }

            return new ChecklistItemDTO
            {
                Id = item.Id,
                Description = item.Description,
                IsCheck = item.IsCheck,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
        }
    }
}
