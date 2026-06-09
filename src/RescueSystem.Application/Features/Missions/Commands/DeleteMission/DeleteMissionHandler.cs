using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.Missions.Commands.DeleteMission
{
    public class DeleteMissionHandler : IRequestHandler<DeleteMissionCommand, bool>
    {
        private readonly IMissionRepository _missionRepository;
        private readonly IRequestRespository _requestRepository;
        private readonly IRescueTeamRepository _rescueTeamRepository;

        public DeleteMissionHandler(
            IMissionRepository missionRepository,
            IRequestRespository requestRepository,
            IRescueTeamRepository rescueTeamRepository)
        {
            _missionRepository = missionRepository;
            _requestRepository = requestRepository;
            _rescueTeamRepository = rescueTeamRepository;
        }

        public async Task<bool> Handle(DeleteMissionCommand request, CancellationToken cancellationToken)
        {
            var mission = await _missionRepository.GetByIdAsync(request.MissionId);
            if (mission == null)
            {
                throw new NotFoundException("Không tìm thấy nhiệm vụ!");
            }

            // Revert Request status to PENDING if it's currently ACCEPTED or IN_PROGRESS
            var requestEntity = await _requestRepository.GetByIdAsync(mission.RequestId);
            if (requestEntity != null && (requestEntity.Status == RequestStatus.ACCEPTED || requestEntity.Status == RequestStatus.IN_PROGRESS))
            {
                await _requestRepository.UpdateStatusAsync(mission.RequestId, RequestStatus.PENDING);
            }

            // Revert Team status to AVAILABLE if it's ON_MISSION
            var team = await _rescueTeamRepository.GetByIdAsync(mission.RescueTeamId);
            if (team != null && team.Status == TeamStatus.ON_MISSION)
            {
                await _rescueTeamRepository.UpdateTeamStatusAsync(team.Id, TeamStatus.AVAILABLE);
            }

            // Physically delete the mission
            await _missionRepository.DeleteAsync(request.MissionId);

            return true;
        }
    }
}
