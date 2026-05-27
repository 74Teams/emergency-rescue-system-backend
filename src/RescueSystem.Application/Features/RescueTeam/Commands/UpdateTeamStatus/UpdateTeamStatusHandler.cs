using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;

namespace RescueSystem.Application.Features.RescueTeam.Commands.UpdateTeamStatus
{
    public class UpdateTeamStatusHandler : IRequestHandler<UpdateTeamStatusCommand, bool>
    {
        private readonly IRescueTeamRepository _rescueTeamRepository;

        public UpdateTeamStatusHandler(IRescueTeamRepository rescueTeamRepository)
        {
            _rescueTeamRepository = rescueTeamRepository;
        }

        public async Task<bool> Handle(UpdateTeamStatusCommand request, CancellationToken cancellationToken)
        {
            return await _rescueTeamRepository.UpdateTeamStatusAsync(request.TeamId, request.NewStatus);
        }
    }
}