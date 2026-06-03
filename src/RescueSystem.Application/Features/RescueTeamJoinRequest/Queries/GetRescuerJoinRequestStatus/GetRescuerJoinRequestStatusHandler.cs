using MediatR;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.RescueTeamJoinRequest.Queries.GetRescuerJoinRequestStatus
{
    public class GetRescuerJoinRequestStatusHandler : IRequestHandler<GetRescuerJoinRequestStatusQuery, RescueTeamJoinRequestDTO?>
    {
        private readonly IRescueTeamJoinRequestRepository _joinRequestRepository;

        public GetRescuerJoinRequestStatusHandler(IRescueTeamJoinRequestRepository joinRequestRepository)
        {
            _joinRequestRepository = joinRequestRepository;
        }

        public async Task<RescueTeamJoinRequestDTO?> Handle(GetRescuerJoinRequestStatusQuery request, CancellationToken cancellationToken)
        {
            var pending = await _joinRequestRepository.GetPendingByRescuerIdAsync(request.RescuerId);
            if (pending == null)
            {
                return null;
            }

            var detailed = await _joinRequestRepository.GetByIdAsync(pending.Id);
            if (detailed == null)
            {
                return null;
            }

            return new RescueTeamJoinRequestDTO
            {
                Id = detailed.Id,
                RescuerId = detailed.RescuerId,
                RescuerName = detailed.Rescuer?.FullName ?? string.Empty,
                RescuerEmail = detailed.Rescuer?.Email ?? string.Empty,
                RescuerPhone = detailed.Rescuer?.PhoneNumber ?? string.Empty,
                RescuerAvatar = detailed.Rescuer?.Avatar ?? string.Empty,
                RescueTeamId = detailed.RescueTeamId,
                TeamName = detailed.RescueTeam?.TeamName ?? string.Empty,
                Status = detailed.Status,
                Message = detailed.Message,
                CreatedAt = detailed.CreatedAt
            };
        }
    }
}
