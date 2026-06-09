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
                Request = m.Request != null ? new RescueSystem.Application.DTOs.Request.RequestBriefDTO
                {
                    Id = m.Request.Id,
                    EmergencyType = m.Request.EmergencyType,
                    Priority = m.Request.Priority,
                    Status = m.Request.Status,
                    Description = m.Request.Description,
                    PhoneNumber = m.Request.PhoneNumber,
                    Location = m.Request.Location != null ? new RescueSystem.Application.DTOs.Location.LocationDTO
                    {
                        Id = m.Request.Location.Id,
                        Address = m.Request.Location.Address,
                        Latitude = m.Request.Location.Latitude,
                        Longitude = m.Request.Location.Longitude
                    } : null
                } : null,
                StartTime = m.StartTime.AddHours(7),
                EndTime = m.EndTime.HasValue ? m.EndTime.Value.AddHours(7) : null,
                CreateAt = m.CreatedAt.AddHours(7),
                UpdateAt = m.UpdatedAt.AddHours(7),
                Status = m.Status
            }).ToList();
        }
    }
}