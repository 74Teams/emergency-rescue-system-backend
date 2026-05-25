using MediatR;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Application.Features.RescueTeam.Commands.UpdateTeamStatus
{
    public class UpdateTeamStatusCommand : IRequest<bool>
    {
        public Guid TeamId { get; set; }
        public TeamStatus NewStatus { get; set; }

        public string? Description { get; set; }
    }
}