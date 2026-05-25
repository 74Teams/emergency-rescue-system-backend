using MediatR;
using Microsoft.AspNetCore.Http;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Features.Request.Commands.ChangeRequestStatus
{
    public class ChangeRequestStatusHandler : IRequestHandler<ChangeRequestStatusCommand, bool>
    {
        private readonly IRequestRespository _requestRepository;

        public ChangeRequestStatusHandler(IRequestRespository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<bool> Handle(ChangeRequestStatusCommand request, CancellationToken cancellationToken)
        {
            var existingRequest = await _requestRepository.GetByIdAsync(request.RequestId);
            if (existingRequest == null)
            {
                throw new NotFoundException("Không tìm thấy yêu cầu cứu hộ");
            }
            var previousStatus = existingRequest.Status;
            existingRequest.Status = request.NewStatus;
            existingRequest.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(request.Note))
            {
                existingRequest.Description += $" [Status Change: {previousStatus} → {request.NewStatus}. Note: {request.Note}]";
            }

            await _requestRepository.UpdateAsync(existingRequest);
            return true;
        }
    }
}
