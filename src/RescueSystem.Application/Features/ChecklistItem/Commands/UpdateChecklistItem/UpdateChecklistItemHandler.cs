using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.ChecklistItem.Commands.UpdateChecklistItem
{
    public class UpdateChecklistItemHandler : IRequestHandler<UpdateChecklistItemCommand, Unit>
    {
        private readonly IChecklistItemRepository _itemRepository;

        public UpdateChecklistItemHandler(
            IChecklistItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<Unit> Handle(UpdateChecklistItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _itemRepository
                .GetByIdAsync(request.Id);

            if (item == null)
            {
                throw new Exception("Không tìm thấy checklist item");
            }

            item.Description = request.Description;
            item.IsCheck = request.IsCheck;
            item.UpdatedAt = DateTime.UtcNow;

            _itemRepository.Update(item);
            await _itemRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
