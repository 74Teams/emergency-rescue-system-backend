using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Mission;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Application.DTOs.User;

namespace RescueSystem.Application.Features.RescueTeam.Queries.GetMissionsByTeamId
{
    public class GetMissionsByTeamIdHandler : IRequestHandler<GetMissionsByTeamIdQuery, List<MissionDTO>>
    {
        private readonly IRescueTeamRepository _rescueTeamRepository;

        public GetMissionsByTeamIdHandler(IRescueTeamRepository rescueTeamRepository)
        {
            _rescueTeamRepository = rescueTeamRepository;
        }

        public async Task<List<MissionDTO>> Handle(GetMissionsByTeamIdQuery request, CancellationToken cancellationToken)
        {
            var missions = await _rescueTeamRepository.GetMissionsByTeamIdAsync(request.TeamId);
            return missions.Select(m => new MissionDTO
            {
                Id = m.Id,
                RequestId = m.RequestId,
                Description = m.Request?.Description,
                Dispatcher = m.Dispatcher != null ? new UserDTO
                {
                    Id = m.DispatcherId,
                    FullName = m.Dispatcher.FullName
                } : null,

                RescueTeam = m.RescueTeam != null ? new RescueTeamDTO
                {
                    Id = m.RescueTeam.Id,
                    TeamName = m.RescueTeam.TeamName,
                    Status = m.RescueTeam.Status.ToString()
                } : null,
                StartTime = m.StartTime.AddHours(7),
                EndTime = m.EndTime.HasValue ? m.EndTime.Value.AddHours(7) : null,
                CreateAt = m.CreatedAt.AddHours(7),
                UpdateAt = m.UpdatedAt.AddHours(7),
                Status = m.Request!.Status
            }).ToList();
        }
    }
}