using MediatR;
using RescueSystem.Application.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.ChecklistItem.Commands.CreateChecklistItem
{
    public class CreateChecklistItemHandler : IRequestHandler<CreateChecklistItemCommand, Guid>
    {
        private readonly IChecklistItemRepository _itemRepository;
        public CreateChecklistItemHandler(IChecklistItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task<Guid> Handle(CreateChecklistItemCommand request, CancellationToken cancellationToken)
        {
            var item = new Domain.Entities.ChecklistItem
            {
                ChecklistId = request.ChecklistId,
                Description = request.Description,
                IsCheck = false
            };

            await _itemRepository.AddAsync(item);
            await _itemRepository.SaveChangesAsync(cancellationToken);

            return item.Id;
        }
    }

}