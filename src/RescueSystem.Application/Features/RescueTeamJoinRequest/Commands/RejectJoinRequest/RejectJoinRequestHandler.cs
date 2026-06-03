using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.RescueTeamJoinRequest.Commands.RejectJoinRequest
{
    public class RejectJoinRequestHandler : IRequestHandler<RejectJoinRequestCommand, bool>
    {
        private readonly IRescueTeamJoinRequestRepository _joinRequestRepository;
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IUserRepository _userRepository;

        public RejectJoinRequestHandler(
            IRescueTeamJoinRequestRepository joinRequestRepository,
            IRescueTeamRepository rescueTeamRepository,
            IUserRepository userRepository)
        {
            _joinRequestRepository = joinRequestRepository;
            _rescueTeamRepository = rescueTeamRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(RejectJoinRequestCommand request, CancellationToken cancellationToken)
        {
            // 1. Get the join request
            var joinReq = await _joinRequestRepository.GetByIdAsync(request.RequestId);
            if (joinReq == null)
            {
                throw new NotFoundException("Yêu cầu gia nhập không tồn tại.");
            }

            if (joinReq.Status != 0)
            {
                throw new BadRequestException("Yêu cầu gia nhập này đã được xử lý.");
            }

            // 2. Get the team
            var team = await _rescueTeamRepository.GetByIdAsync(joinReq.RescueTeamId);
            if (team == null)
            {
                throw new NotFoundException("Đội cứu hộ không tồn tại.");
            }

            // 3. Authorize: must be Commander OR Team Leader of the target team
            var rejecterRoles = await _userRepository.GetUserRolesAsync(request.RejectedById);
            bool isCommander = rejecterRoles.Contains("Commander");
            bool isTeamLeader = team.TeamLeaderId == request.RejectedById;

            if (!isCommander && !isTeamLeader)
            {
                throw new UnauthorizedException("Bạn không có quyền từ chối yêu cầu gia nhập này.");
            }

            // 4. Update the join request status to Rejected
            joinReq.Status = 2; // Rejected
            joinReq.UpdatedAt = DateTime.UtcNow;
            await _joinRequestRepository.UpdateAsync(joinReq);

            return true;
        }
    }
}
