using MediatR;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Application.Interfaces.Respositories;

namespace RescueSystem.Application.Features.RescueTeam.Commands.AddMemberToRescueTeam
{
    public class AddMemberToRescueTeamHandler : IRequestHandler<AddMemberToRescueTeamCommand, bool>
    {
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IUserRepository _userRepository;


        public AddMemberToRescueTeamHandler(IRescueTeamRepository rescueTeamRepository, IUserRepository userRepository)
        {
            _rescueTeamRepository = rescueTeamRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(AddMemberToRescueTeamCommand request, CancellationToken cancellationToken)
        {
            var isAdded = await _rescueTeamRepository.AddMemberAsync(request.TeamId, request.MemberId);
            if (!isAdded) return false;

            var currentRoles = await _userRepository.GetUserRolesAsync(request.MemberId);
            if (!currentRoles.Contains("Rescuer"))
            {
                var rolesList = currentRoles.ToList();
                rolesList.Add("Rescuer");
                await _userRepository.UpdateUserRolesAsync(request.MemberId, rolesList);
            }
            return true;
        }
    }
}