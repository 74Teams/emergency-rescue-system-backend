using MediatR;
using RescueSystem.Application.DTOs.Mission;

namespace RescueSystem.Application.Features.RescueTeam.Queries.GetMissionsByTeamId
{
    public class GetMissionsByTeamIdQuery : IRequest<List<MissionDTO>>
    {
        public Guid TeamId { get; set; }
    }
}