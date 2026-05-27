using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.ChecklistItem.Commands.DeleteChecklistItem
{
    public class DeleteChecklistItemHandler : IRequestHandler<DeleteChecklistItemCommand, Unit>
    {
        private readonly IChecklistItemRepository _itemRepository;

        public DeleteChecklistItemHandler(IChecklistItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<Unit> Handle(DeleteChecklistItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _itemRepository.GetByIdAsync(request.Id);

            if (item == null)
            {
                throw new Exception("Không tìm thấy checklist item");
            }

            _itemRepository.Delete(item);

            await _itemRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
