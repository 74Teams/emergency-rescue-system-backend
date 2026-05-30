using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;
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
                throw new Exception("Không tìm thấy nhiệm vụ!");
            }
            // khoong cho abort khi nvu đã hoàn thành hoặc đã bị hủy
            if (mission.Status == MissionStatus.COMPLETED || mission.Status == MissionStatus.ABORTED)
            {
                throw new Exception("Nhiệm vụ đã kết thúc, không thể hủy.");
            }

            var previousStatus = mission.Status;
            mission.Status = MissionStatus.ABORTED;
            mission.EndTime = DateTime.UtcNow;
            mission.UpdatedAt = DateTime.UtcNow;


            var history = new MissionHistory
            {
                MissionId = mission.Id,
                FromStatus = previousStatus,
                ToStatus = mission.Status,
                ChangedById = request.ChangedById,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow //EDIT: 30/5 by Dieu - Đồng bộ UTC
            };

            await _missionRepository.UpdateAsync(mission);
            await _missionRepository.AddHistoryAsync(history);

            // Update Request and Team status on abort
            await _requestRepository.UpdateStatusAsync(mission.RequestId, RequestStatus.CANCELED);

            var team = await _rescueTeamRepository.GetByIdAsync(mission.RescueTeamId);
            if (team != null)
            {
                team.Status = TeamStatus.AVAILABLE;
                team.UpdatedAt = DateTime.UtcNow; //EDIT: 30/5 by Dieu - Đồng bộ UTC
                await _rescueTeamRepository.UpdateTeamStatusAsync(team.Id, TeamStatus.AVAILABLE);
            }

            return true;
        }
    }
}
