using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Checklist.Queries.GetChecklistDetail
{
    public class GetChecklistDetailHandler : IRequestHandler<GetChecklistDetailQuery, ChecklistDetailDTO>
    {
        private readonly IChecklistRepository _checklistRepository;

        public GetChecklistDetailHandler(IChecklistRepository checklistRepository)
        {
            _checklistRepository = checklistRepository;
        }

        public async Task<ChecklistDetailDTO> Handle(GetChecklistDetailQuery request, CancellationToken cancellationToken)
        {
            var checklist = await _checklistRepository.GetByIdAsync(request.Id);

            if (checklist == null)
            {
                throw new Exception("Không tìm thấy Checklist");
            }
            return new ChecklistDetailDTO
            {
                Id = checklist.Id,
                Title = checklist.Title,
                MissionId = checklist.MissionId,
                CreatedAt = checklist.CreatedAt,
                UpdatedAt = checklist.UpdatedAt,

                Items = checklist.ChecklistItems.Select(i => new ChecklistItemDTO
                {
                    Id = i.Id,
                    Description = i.Description,
                    IsCheck = i.IsCheck,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                }).ToList()
            };
        }
    }
}
