using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Features.Request.Commands.DeleteRequest
{
    public class DeleteRequestHandler : IRequestHandler<DeleteRequestCommand, bool>
    {
        private readonly IRequestRespository _requestRepository;

        public DeleteRequestHandler(IRequestRespository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<bool> Handle(DeleteRequestCommand request, CancellationToken cancellationToken)
        {
            var requestEntity = await _requestRepository.GetByIdAsync(request.RequestId);
            if (requestEntity == null)
            {
                throw new NotFoundException("Không tìm thấy yêu cầu cứu hộ");
            }

            if (requestEntity.Missions != null && requestEntity.Missions.Count > 0)
            {
                throw new BadRequestException("Không thể xóa yêu cầu cứu hộ đang liên kết với nhiệm vụ.");
            }

            await _requestRepository.DeleteAsync(requestEntity);
            return true;
        }
    }
}
