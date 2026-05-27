using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;

namespace RescueSystem.Application.Features.Location.Commands.DeleteLocation
{
    public class DeleteLocationHandler : IRequestHandler<DeleteLocationCommand, bool>
    {
        private readonly ILocationRepository _locationRepository;

        public DeleteLocationHandler(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<bool> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
        {
            var location = await _locationRepository.GetByIdAsync(request.Id);
            if (location == null)
            {
                throw new NotFoundException("Không tìm thấy vị trí");
            }

            return await _locationRepository.DeleteAsync(location);
        }
    }
}
