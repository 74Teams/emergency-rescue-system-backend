using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.LeaveRequest;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.LeaveRequest.Commands.CreateLeaveRequest
{
    public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, LeaveRequestDTO>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IUserRepository _userRepository;

        public CreateLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository, IUserRepository userRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _userRepository = userRepository;
        }

        public async Task<LeaveRequestDTO> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var rescuer = await _userRepository.GetUserByIdAsync(request.RescuerId.ToString());
            if (rescuer == null)
                throw new NotFoundException("Rescuer not found");

            if (!rescuer.RescueTeamId.HasValue)
                throw new BadRequestException("Rescuer is not a member of any rescue team.");

            var leaveRequest = new Domain.Entities.LeaveRequest
            {
                RescuerId = request.RescuerId,
                RescueTeamId = rescuer.RescueTeamId.Value,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Reason = request.Reason,
                Status = 0 // Pending
            };

            await _leaveRequestRepository.CreateAsync(leaveRequest);

            var createdReq = await _leaveRequestRepository.GetByIdAsync(leaveRequest.Id);

            return LeaveRequestDTO.FromEntity(createdReq);
        }
    }
}
