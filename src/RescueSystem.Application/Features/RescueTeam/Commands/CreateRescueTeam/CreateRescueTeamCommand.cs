using MediatR;
using RescueSystem.Application.DTOs.RescueTeam;

namespace RescueSystem.Application.Features.RescueTeam.Commands.CreateRescueTeam
{
    public class CreateRescueTeamCommand : IRequest<RescueTeamDTO>
    {
        public string TeamName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid TeamLeaderId { get; set; }
        public Guid BaseLocationId { get; set; }
    }
}