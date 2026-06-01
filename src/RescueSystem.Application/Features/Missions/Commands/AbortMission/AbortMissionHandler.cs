using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;
using RescueSystem.Application.Common.Exception;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.Missions.Commands.AbortMission
{
    public class AbortMissionHandler : IRequestHandler<AbortMissionCommand, bool>
    {
        private readonly IMissionRepository _missionRepository;
        private readonly IRequestRespository _requestRepository;
        private readonly IRescueTeamRepository _rescueTeamRepository;

        public AbortMissionHandler(
            IMissionRepository missionRepository,
            IRequestRespository requestRepository,
            IRescueTeamRepository rescueTeamRepository)
        {
            _missionRepository = missionRepository;
            _requestRepository = requestRepository;
            _rescueTeamRepository = rescueTeamRepository;
        }

        public async Task<bool> Handle(AbortMissionCommand request, CancellationToken cancellationToken)
        {
            var mission = await _missionRepository.GetByIdAsync(request.MissionId);
            if (mission == null)
            {
                throw new NotFoundException("Không tìm thấy nhiệm vụ!");
            }
            if (mission.Status == MissionStatus.COMPLETED || mission.Status == MissionStatus.ABORTED)
            {
                throw new BadRequestException("Nhiệm vụ đã kết thúc, không thể hủy.");
            }

            var previousStatus = mission.Status;
            mission.Status = MissionStatus.ABORTED;
            mission.EndTime = DateTime.UtcNow;
            mission.UpdatedAt = DateTime.UtcNow;


            Guid changedByGuid = Guid.Empty;
            if (!string.IsNullOrEmpty(request.ChangedById) && Guid.TryParse(request.ChangedById, out var parsedGuid))
            {
                changedByGuid = parsedGuid;
            }

            var history = new MissionHistory
            {
                MissionId = mission.Id,
                FromStatus = previousStatus,
                ToStatus = mission.Status,
                ChangedById = changedByGuid == Guid.Empty ? mission.DispatcherId : changedByGuid,
                Note = string.IsNullOrWhiteSpace(request.Note) ? "Hủy bỏ nhiệm vụ" : request.Note,
                CreatedAt = DateTime.UtcNow 
            };

            await _missionRepository.UpdateAsync(mission);
            await _missionRepository.AddHistoryAsync(history);

            // Update Request and Team status on abort
            await _requestRepository.UpdateStatusAsync(mission.RequestId, RequestStatus.CANCELED);

            var team = await _rescueTeamRepository.GetByIdAsync(mission.RescueTeamId);
            if (team != null)
            {
                team.Status = TeamStatus.AVAILABLE;
                team.UpdatedAt = DateTime.UtcNow; 
                await _rescueTeamRepository.UpdateTeamStatusAsync(team.Id, TeamStatus.AVAILABLE);
            }

            return true;
        }
    }
}
