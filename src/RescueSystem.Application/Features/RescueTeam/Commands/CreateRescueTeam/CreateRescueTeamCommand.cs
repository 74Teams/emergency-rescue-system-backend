using MediatR;
using RescueSystem.Application.DTOs.RescueTeam;

namespace RescueSystem.Application.Features.RescueTeam.Command.CreateRescueTeam
{
    public class CreateRescueTeamCommand : IRequest<RescueTeamDTO>
    {
        public string TeamName { get; set; } = string.Empty;
        public Guid TeamLeaderId { get; set; }
        public Guid BaseLocationId { get; set; }
    }
}