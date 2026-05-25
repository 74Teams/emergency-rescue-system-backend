using MediatR;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Application.Interfaces.Respositories;
using System.Linq;

namespace RescueSystem.Application.Features.RescueTeam.Commands.DeleteRescueTeam
{
    public class DeleteRescueTeamHandler : IRequestHandler<DeleteRescueTeamCommand, bool>
    {
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IUserRepository _userRepository;
        public DeleteRescueTeamHandler(
            IRescueTeamRepository rescueTeamRepository,
            IUserRepository userRepository)
        {
            _rescueTeamRepository = rescueTeamRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(DeleteRescueTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await _rescueTeamRepository.GetByIdAsync(request.Id);
            if (team == null)
            {
                throw new KeyNotFoundException("Rescue team not found");
            }

            var leaderId = team.TeamLeaderId;

            var isDeleted = await _rescueTeamRepository.DeleteAsync(team);

            if (!isDeleted)
            {
                return false;
            }

            var currentRoles = await _userRepository.GetUserRolesAsync(leaderId);
            var rolesList = currentRoles.ToList();

            if (rolesList.Contains("RescuerLeader"))
            {
                rolesList.Remove("RescuerLeader");
                await _userRepository.UpdateUserRolesAsync(leaderId, rolesList);
            }
            // ===================================================

            return true;
        }
    }
}