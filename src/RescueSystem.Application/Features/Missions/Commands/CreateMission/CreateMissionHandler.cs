using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.Missions.Commands.CreateMission
{
    public class CreateMissionHandler : IRequestHandler<CreateMissionCommand, Guid>
    {
        private readonly IMissionRepository _missionRepository;
        private readonly IRescueTeamRepository _rescueTeamRepository; // FIXED: Added

        public CreateMissionHandler(IMissionRepository missionRepository, IRescueTeamRepository rescueTeamRepository)
        {
            _missionRepository = missionRepository;
            _rescueTeamRepository = rescueTeamRepository;
        }
        public async Task<Guid> Handle(CreateMissionCommand request, CancellationToken cancellationToken)
        {
            var existingMission = await _missionRepository
                .GetByRequestAndTeamAsync(request.RequestId, request.RescueTeamId);
            if (existingMission != null
                && existingMission.Status != MissionStatus.COMPLETED
                && existingMission.Status != MissionStatus.ABORTED
                )
            {
                throw new BadRequestException("Team này đã được assign cho request này rồi");
            }

            var busyMission = await _missionRepository
                .GetActiveMissionByTeamIdAsync(request.RescueTeamId);
            if (busyMission != null)
            {
                throw new BadRequestException("Team này đang bận với một mission khác");
            }

            var mission = new Mission
            {
                Id = Guid.NewGuid(),
                RequestId = request.RequestId,
                DispatcherId = request.DispatcherId,
                RescueTeamId = request.RescueTeamId,
                StartTime = DateTime.UtcNow.AddHours(7),
                Status = Domain.Enums.MissionStatus.ASSIGNED,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                UpdatedAt = DateTime.UtcNow.AddHours(7),
            };

            var res = await _missionRepository.AddAsync(mission);

            var history = new MissionHistory
            {
                MissionId = mission.Id,
                FromStatus = null,
                ToStatus = mission.Status,
                ChangedById = request.DispatcherId,
                Note = "Dispatcher created mission",
                CreatedAt = DateTime.UtcNow.AddHours(7)
            };
            await _missionRepository.AddHistoryAsync(history);

            return mission.Id;
        }
    }
}
