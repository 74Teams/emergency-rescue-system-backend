using MediatR;
using RescueSystem.Application.Common.Interfaces.Repositories;
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

        public AbortMissionHandler(IMissionRepository missionRepository)
        {
            _missionRepository = missionRepository;
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
                CreatedAt = DateTime.UtcNow.AddHours(7)
            };

            await _missionRepository.UpdateAsync(mission);
            await _missionRepository.AddHistoryAsync(history);

            return true;
        }
    }
}
