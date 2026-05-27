using AutoMapper;
using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Location;
using LocationEntity = RescueSystem.Domain.Entities.Location;

namespace RescueSystem.Application.Features.Location.Commands.CreateLocation
{
    public class CreateLocationHandler : IRequestHandler<CreateLocationCommand, LocationDTO>
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public CreateLocationHandler(ILocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public async Task<LocationDTO> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        {
            var entity = new LocationEntity
            {
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Address = request.Address,
                Landmark = request.Landmark
            };

            var result = await _locationRepository.CreateAsync(entity);
            if (!result)
            {
                throw new System.Exception("Không thể tạo vị trí");
            }

            return _mapper.Map<LocationDTO>(entity);
        }
    }
}
