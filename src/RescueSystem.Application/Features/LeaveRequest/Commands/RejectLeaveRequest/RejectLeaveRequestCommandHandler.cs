using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.LeaveRequest;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.LeaveRequest.Commands.RejectLeaveRequest
{
    public class RejectLeaveRequestCommandHandler : IRequestHandler<RejectLeaveRequestCommand, LeaveRequestDTO>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public RejectLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<LeaveRequestDTO> Handle(RejectLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.LeaveRequestId);
            if (leaveRequest == null)
            {
                throw new NotFoundException("Không tìm thấy yêu cầu xin nghỉ phép này.");
            }

            if (leaveRequest.Status != 0)
            {
                throw new BadRequestException("Yêu cầu này đã được xử lý trước đó.");
            }

            leaveRequest.Status = 2; // Rejected
            if (!string.IsNullOrEmpty(request.Note))
            {
                leaveRequest.Note = request.Note;
            }

            await _leaveRequestRepository.UpdateAsync(leaveRequest);

            return LeaveRequestDTO.FromEntity(leaveRequest);
        }
    }
}
