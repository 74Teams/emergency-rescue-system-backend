using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Checklist.Commands.CreateChecklist
{
    public class CreateChecklistHandler : IRequestHandler<CreateChecklistCommand, Guid>
    {
        private readonly IChecklistRepository _checklistRepository;

        public CreateChecklistHandler(IChecklistRepository checklistRepository)
        {
            _checklistRepository = checklistRepository;
        }

        public async Task<Guid> Handle(CreateChecklistCommand request, CancellationToken cancellationToken)
        {
            var checklist = new Domain.Entities.Checklist
            {
                Title = request.Title,
                MissionId = request.MissionId
            };

            await _checklistRepository.AddAsync(checklist);
            await _checklistRepository.SaveChangesAsync(cancellationToken);

            return checklist.Id;
        }
    }
}
