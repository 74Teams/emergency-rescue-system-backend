using MediatR;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Application.Features.Checklist;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.ChecklistItem.Queries.GetChecklistItems
{
    public class GetChecklistItemsHandler : IRequestHandler<GetChecklistItemsQuery, List<ChecklistItemDTO>>
    {
        private readonly IChecklistItemRepository _itemRepository;
        public GetChecklistItemsHandler(IChecklistItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task<List<ChecklistItemDTO>> Handle(GetChecklistItemsQuery request, CancellationToken cancellationToken)
        {
            var items = await _itemRepository.GetByChecklistIdAsync(request.ChecklistId);

            return items.Select(x => new ChecklistItemDTO
            {
                Id = x.Id,
                Description = x.Description,
                IsCheck = x.IsCheck,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).ToList();
        }
    }
}
