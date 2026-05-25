using MediatR;
using RescueSystem.Application.Common.Response;

namespace RescueSystem.Application.Features.RescueTeam.Commands.AssignTeamLeader
{
    public class AssignTeamLeaderCommand : IRequest<ApiResponse<object>>
    {
        public Guid TeamId {get; set;}
        public Guid UserId {get; set;}
    }
}