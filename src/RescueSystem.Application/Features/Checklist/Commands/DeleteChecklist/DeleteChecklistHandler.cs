using MediatR;
using RescueSystem.Application.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Checklist.Commands.DeleteChecklist
{
    public class DeleteChecklistHandler : IRequestHandler<DeleteChecklistCommand, Unit>
    {
        private readonly IChecklistRepository _checklistRepository;

        public DeleteChecklistHandler(
            IChecklistRepository checklistRepository)
        {
            _checklistRepository = checklistRepository;
        }

        public async Task<Unit> Handle(DeleteChecklistCommand request, CancellationToken cancellationToken)
        {
            var checklist = await _checklistRepository.GetByIdAsync(request.Id);

            if (checklist == null)
            {
                throw new Exception("Không tìm thấy checklist");
            }

            _checklistRepository.Delete(checklist);

            await _checklistRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
