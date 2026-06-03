using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.RescueTeamJoinRequest.Commands.CreateJoinRequest
{
    public class CreateJoinRequestHandler : IRequestHandler<CreateJoinRequestCommand, RescueTeamJoinRequestDTO>
    {
        private readonly IRescueTeamJoinRequestRepository _joinRequestRepository;
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IUserRepository _userRepository;

        public CreateJoinRequestHandler(
            IRescueTeamJoinRequestRepository joinRequestRepository,
            IRescueTeamRepository rescueTeamRepository,
            IUserRepository userRepository)
        {
            _joinRequestRepository = joinRequestRepository;
            _rescueTeamRepository = rescueTeamRepository;
            _userRepository = userRepository;
        }

        public async Task<RescueTeamJoinRequestDTO> Handle(CreateJoinRequestCommand request, CancellationToken cancellationToken)
        {
            // 1. Check if user exists
            var user = await _userRepository.GetUserByIdAsync(request.RescuerId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng");
            }

            // 2. Check if user is already assigned to a team
            if (user.RescueTeamId.HasValue && user.RescueTeamId != Guid.Empty)
            {
                throw new BadRequestException("Cứu hộ viên đã tham gia vào một đội cứu hộ khác.");
            }

            // 3. Verify user has Rescuer role
            var roles = await _userRepository.GetUserRolesAsync(request.RescuerId);
            if (!roles.Contains("Rescuer"))
            {
                throw new BadRequestException("Chỉ người dùng có vai trò Cứu hộ viên mới được phép xin gia nhập đội.");
            }

            // 4. Check if team exists
            var team = await _rescueTeamRepository.GetByIdAsync(request.RescueTeamId);
            if (team == null)
            {
                throw new NotFoundException("Không tìm thấy đội cứu hộ được chọn.");
            }

            // 5. Check if user already has a pending join request
            var pendingRequest = await _joinRequestRepository.GetPendingByRescuerIdAsync(request.RescuerId);
            if (pendingRequest != null)
            {
                throw new BadRequestException("Bạn đã có một yêu cầu xin gia nhập đội đang chờ duyệt.");
            }

            // 6. Create join request
            var joinReq = new RescueSystem.Domain.Entities.RescueTeamJoinRequest
            {
                Id = Guid.NewGuid(),
                RescuerId = request.RescuerId,
                RescueTeamId = request.RescueTeamId,
                Message = request.Message,
                Status = 0, // Pending
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _joinRequestRepository.CreateAsync(joinReq);

            // Fetch request again to load related entities
            var createdReq = await _joinRequestRepository.GetByIdAsync(joinReq.Id);
            if (createdReq == null)
            {
                throw new Exception("Lỗi hệ thống khi tạo yêu cầu gia nhập.");
            }

            return new RescueTeamJoinRequestDTO
            {
                Id = createdReq.Id,
                RescuerId = createdReq.RescuerId,
                RescuerName = createdReq.Rescuer?.FullName ?? string.Empty,
                RescuerEmail = createdReq.Rescuer?.Email ?? string.Empty,
                RescuerPhone = createdReq.Rescuer?.PhoneNumber ?? string.Empty,
                RescuerAvatar = createdReq.Rescuer?.Avatar ?? string.Empty,
                RescueTeamId = createdReq.RescueTeamId,
                TeamName = createdReq.RescueTeam?.TeamName ?? string.Empty,
                Status = createdReq.Status,
                Message = createdReq.Message,
                CreatedAt = createdReq.CreatedAt
            };
        }
    }
}
