using AutoMapper;
using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Location;
using LocationEntity = RescueSystem.Domain.Entities.Location;

namespace RescueSystem.Application.Features.Location.Commands.UpdateLocation
{
    public class UpdateLocationHandler : IRequestHandler<UpdateLocationCommand, LocationDTO>
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public UpdateLocationHandler(ILocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public async Task<LocationDTO> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
        {
            var location = await _locationRepository.GetByIdAsync(request.Id);
            if (location == null)
            {
                throw new NotFoundException("Không tìm thấy vị trí");
            }

            location.Latitude = request.Latitude;
            location.Longitude = request.Longitude;
            location.Address = request.Address;
            location.Landmark = request.Landmark;
            location.UpdatedAt = DateTime.UtcNow;

            var result = await _locationRepository.UpdateAsync(location);
            if (!result)
            {
                throw new System.Exception("Không thể cập nhật vị trí");
            }

            return _mapper.Map<LocationDTO>(location);
        }
    }
}
