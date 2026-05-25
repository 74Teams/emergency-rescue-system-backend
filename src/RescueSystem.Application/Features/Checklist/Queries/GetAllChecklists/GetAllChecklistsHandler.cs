using MediatR;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Checklist;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Checklist.Queries.GetAllChecklists
{
    public class GetAllChecklistsHandler : IRequestHandler<GetAllChecklistsQuery, List<ChecklistDTO>>
    {
        private readonly IChecklistRepository _checkListRepository;

        public GetAllChecklistsHandler(IChecklistRepository checkListRepository)
        {
            _checkListRepository = checkListRepository;
        }

        public async Task<List<ChecklistDTO>> Handle(GetAllChecklistsQuery request, CancellationToken cancellationToken)
        {
            var checklists = await _checkListRepository.GetAllAsync();

            return checklists.Select(c => new ChecklistDTO
            {
                Id = c.Id,
                Title = c.Title,
                MissionId = c.MissionId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();
        }
    }
}
