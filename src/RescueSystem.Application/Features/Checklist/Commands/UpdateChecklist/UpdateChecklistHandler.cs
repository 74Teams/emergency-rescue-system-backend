using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Checklist.Commands.UpdateChecklist
{
    public class UpdateChecklistHandler : IRequestHandler<UpdateChecklistCommand, Unit>
    {
        private readonly IChecklistRepository _checklistRepository;

        public UpdateChecklistHandler(IChecklistRepository checklistRepository)
        {
            _checklistRepository = checklistRepository;
        }

        public async Task<Unit> Handle(UpdateChecklistCommand request, CancellationToken cancellationToken)
        {
            var checklist = await _checklistRepository
                .GetByIdAsync(request.Id);

            if (checklist == null)
            {
                throw new Exception("Không tìm thấy checklist");
            }

            checklist.Title = request.Title;
            checklist.UpdatedAt = DateTime.UtcNow;
            _checklistRepository.Update(checklist);

            await _checklistRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
