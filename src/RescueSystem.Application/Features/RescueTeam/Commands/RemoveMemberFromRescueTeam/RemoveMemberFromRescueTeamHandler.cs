using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;

namespace RescueSystem.Application.Features.RescueTeam.Commands.RemoveMemberFromRescueTeam
{
    public class RemoveMemberFromRescueTeamHandler : IRequestHandler<RemoveMemberFromRescueTeamCommand, bool>
    {
        private readonly IRescueTeamRepository _rescueTeamRepository;
        public RemoveMemberFromRescueTeamHandler(IRescueTeamRepository rescueTeamRepository)
        {
            _rescueTeamRepository = rescueTeamRepository;
        }

        public async Task<bool> Handle(RemoveMemberFromRescueTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await _rescueTeamRepository.GetByIdAsync(request.TeamId);
            if (team != null && team.TeamLeaderId == request.MemberId)
            {
                throw new BadRequestException("Không thể đuổi Đội trưởng ra khỏi đội. Vui lòng chuyển giao chức Đội trưởng hoặc giải tán đội trước!");
            }

            return await _rescueTeamRepository.RemoveMemberAsync(request.TeamId, request.MemberId);
        }
    }
}