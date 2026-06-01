using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.Missions.Commands.AddMissionHistory
{
    public class AddMissionHistoryHandler : IRequestHandler<AddMissionHistoryCommand, bool>
    {
        private readonly IMissionRepository _missionRepository;

        public AddMissionHistoryHandler(IMissionRepository missionRepository)
        {
            _missionRepository = missionRepository;
        }

        public async Task<bool> Handle(AddMissionHistoryCommand request, CancellationToken cancellationToken)
        {
            var mission = await _missionRepository.GetByIdAsync(request.MissionId);
            if (mission == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ!");
            }

            var history = new MissionHistory
            {
                MissionId = mission.Id,
                FromStatus = mission.Status,
                ToStatus = mission.Status, // status unchanged
                ChangedById = request.ChangedById,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow
            };

            await _missionRepository.AddHistoryAsync(history);

            return true;
        }
    }
}
