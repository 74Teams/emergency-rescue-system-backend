using MediatR;

namespace RescueSystem.Application.Features.RescueTeam.Commands.RemoveMemberFromRescueTeam
{
    public class RemoveMemberFromRescueTeamCommand : IRequest<bool>
    {
        public Guid TeamId { get; set; }
        public Guid MemberId { get; set; }
    }
}