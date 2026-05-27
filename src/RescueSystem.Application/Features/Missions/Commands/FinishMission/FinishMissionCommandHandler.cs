using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.Missions.Commands.FinishMission
{
    public class FinishMissionCommandHandler : IRequestHandler<FinishMissionCommand, bool>
    {
        private readonly IMissionRepository _missionRepository;
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IRequestRespository _requestRepository;

        public FinishMissionCommandHandler(
            IMissionRepository missionRepository, 
            IRescueTeamRepository rescueTeamRepository,
            IRequestRespository requestRepository)
        {
            _missionRepository = missionRepository;
            _rescueTeamRepository = rescueTeamRepository;
            _requestRepository = requestRepository;
        }
        public async Task<bool> Handle(FinishMissionCommand request, CancellationToken cancellationToken)
        {
            var mission = await _missionRepository.GetByIdAsync(request.MissionId);
            if (mission == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ!");
            }

            if (mission.Status != MissionStatus.IN_PROGRESS)
            {
                throw new Exception("Chỉ có thể hoàn thành khi nhiệm vụ đang IN_PROGRESS");
            }

            var previousStatus = mission.Status;

            mission.Status = MissionStatus.COMPLETED;
            mission.EndTime = DateTime.UtcNow;
            mission.UpdatedAt = DateTime.UtcNow;

            var history = new MissionHistory
            {
                MissionId = mission.Id,
                FromStatus = previousStatus,
                ToStatus = mission.Status,
                ChangedById = request.ChangedById,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow.AddHours(7)
            };

            await _missionRepository.UpdateAsync(mission);
            await _missionRepository.AddHistoryAsync(history);

            var team = await _rescueTeamRepository.GetByIdAsync(mission.RescueTeamId);
            if (team != null)
            {
                team.Status = TeamStatus.AVAILABLE;
                team.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await _rescueTeamRepository.UpdateTeamStatusAsync(team.Id, TeamStatus.AVAILABLE);
            }

            await _requestRepository.UpdateStatusAsync(mission.RequestId, RequestStatus.COMPLETED);

            return true;
        }
    }
}