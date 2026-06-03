using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.RescueTeamJoinRequest.Queries.GetTeamJoinRequests
{
    public class GetTeamJoinRequestsHandler : IRequestHandler<GetTeamJoinRequestsQuery, List<RescueTeamJoinRequestDTO>>
    {
        private readonly IRescueTeamJoinRequestRepository _joinRequestRepository;
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IUserRepository _userRepository;

        public GetTeamJoinRequestsHandler(
            IRescueTeamJoinRequestRepository joinRequestRepository,
            IRescueTeamRepository rescueTeamRepository,
            IUserRepository userRepository)
        {
            _joinRequestRepository = joinRequestRepository;
            _rescueTeamRepository = rescueTeamRepository;
            _userRepository = userRepository;
        }

        public async Task<List<RescueTeamJoinRequestDTO>> Handle(GetTeamJoinRequestsQuery request, CancellationToken cancellationToken)
        {
            var userRoles = await _userRepository.GetUserRolesAsync(request.UserId);
            bool isCommander = userRoles.Contains("Commander");

            Guid targetTeamId;

            if (request.TeamId.HasValue)
            {
                targetTeamId = request.TeamId.Value;
                // If not commander, must check if they are the leader of the target team
                if (!isCommander)
                {
                    var team = await _rescueTeamRepository.GetByIdAsync(targetTeamId);
                    if (team == null)
                    {
                        throw new NotFoundException("Không tìm thấy đội cứu hộ.");
                    }
                    if (team.TeamLeaderId != request.UserId)
                    {
                        throw new UnauthorizedException("Bạn không có quyền xem yêu cầu gia nhập của đội này.");
                    }
                }
            }
            else
            {
                // No team ID provided.
                if (isCommander)
                {
                    // Commander can view ALL pending requests across the system
                    var allPending = await _joinRequestRepository.GetAllPendingAsync();
                    return MapToDTOList(allPending);
                }
                else
                {
                    // Find the team this user leads
                    var allTeams = await _rescueTeamRepository.GetAllAsync();
                    var ledTeam = allTeams.FirstOrDefault(t => t.TeamLeaderId == request.UserId);
                    if (ledTeam == null)
                    {
                        throw new BadRequestException("Bạn không lãnh đạo đội cứu hộ nào.");
                    }
                    targetTeamId = ledTeam.Id;
                }
            }

            var teamRequests = await _joinRequestRepository.GetPendingByTeamIdAsync(targetTeamId);
            return MapToDTOList(teamRequests);
        }

        private List<RescueTeamJoinRequestDTO> MapToDTOList(List<Domain.Entities.RescueTeamJoinRequest> requests)
        {
            return requests.Select(r => new RescueTeamJoinRequestDTO
            {
                Id = r.Id,
                RescuerId = r.RescuerId,
                RescuerName = r.Rescuer?.FullName ?? string.Empty,
                RescuerEmail = r.Rescuer?.Email ?? string.Empty,
                RescuerPhone = r.Rescuer?.PhoneNumber ?? string.Empty,
                RescuerAvatar = r.Rescuer?.Avatar ?? string.Empty,
                RescueTeamId = r.RescueTeamId,
                TeamName = r.RescueTeam?.TeamName ?? string.Empty,
                Status = r.Status,
                Message = r.Message,
                CreatedAt = r.CreatedAt
            }).ToList();
        }
    }
}
