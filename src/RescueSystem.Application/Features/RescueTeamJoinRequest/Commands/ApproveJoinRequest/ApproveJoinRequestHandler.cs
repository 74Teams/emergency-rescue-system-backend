using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.RescueTeamJoinRequest.Commands.ApproveJoinRequest
{
    public class ApproveJoinRequestHandler : IRequestHandler<ApproveJoinRequestCommand, bool>
    {
        private readonly IRescueTeamJoinRequestRepository _joinRequestRepository;
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IUserRepository _userRepository;

        public ApproveJoinRequestHandler(
            IRescueTeamJoinRequestRepository joinRequestRepository,
            IRescueTeamRepository rescueTeamRepository,
            IUserRepository userRepository)
        {
            _joinRequestRepository = joinRequestRepository;
            _rescueTeamRepository = rescueTeamRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(ApproveJoinRequestCommand request, CancellationToken cancellationToken)
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
            var approverRoles = await _userRepository.GetUserRolesAsync(request.ApprovedById);
            bool isCommander = approverRoles.Contains("Commander");
            bool isTeamLeader = team.TeamLeaderId == request.ApprovedById;

            if (!isCommander && !isTeamLeader)
            {
                throw new UnauthorizedException("Bạn không có quyền duyệt yêu cầu gia nhập này.");
            }

            // 4. Update the join request status to Approved
            joinReq.Status = 1; // Approved
            joinReq.UpdatedAt = DateTime.UtcNow;
            await _joinRequestRepository.UpdateAsync(joinReq);

            // 5. Add the member to the rescue team (this updates user's RescueTeamId and saves)
            var added = await _rescueTeamRepository.AddMemberAsync(joinReq.RescueTeamId, joinReq.RescuerId);
            if (!added)
            {
                throw new Exception("Không thể thêm Cứu hộ viên vào đội cứu hộ.");
            }

            return true;
        }
    }
}
