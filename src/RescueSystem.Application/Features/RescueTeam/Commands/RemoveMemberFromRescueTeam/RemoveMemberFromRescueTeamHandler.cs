using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;

namespace RescueSystem.Application.Features.RescueTeam.Commands.RemoveMemberFromRescueTeam
{
    public class RemoveMemberFromRescueTeamHandler : IRequestHandler<RemoveMemberFromRescueTeamCommand, bool>
    {
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IUserRepository _userRepository;
        public RemoveMemberFromRescueTeamHandler(IRescueTeamRepository rescueTeamRepository, IUserRepository userRepository)
        {
            _rescueTeamRepository = rescueTeamRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(RemoveMemberFromRescueTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await _rescueTeamRepository.GetByIdAsync(request.TeamId);
            if (team != null && team.TeamLeaderId == request.MemberId)
            {
                throw new BadRequestException("Không thể đuổi Đội trưởng ra khỏi đội. Vui lòng chuyển giao chức Đội trưởng hoặc giải tán đội trước!");
            }
            var isRemoved = await _rescueTeamRepository.RemoveMemberAsync(request.TeamId, request.MemberId);
            if (!isRemoved) return false;

            var currentRoles = await _userRepository.GetUserRolesAsync(request.MemberId);
            var rolesList = currentRoles.ToList();

            bool removedRescuer = rolesList.Remove("Rescuer");
            bool removedLeader = rolesList.Remove("RescuerLeader");
            if (removedRescuer || removedLeader)
            {
                await _userRepository.UpdateUserRolesAsync(request.MemberId, rolesList);
            }
            return true;
        }
    }
}