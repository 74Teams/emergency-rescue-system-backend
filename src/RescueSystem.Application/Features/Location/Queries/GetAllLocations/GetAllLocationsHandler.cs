using AutoMapper;
using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Location;

namespace RescueSystem.Application.Features.Location.Queries.GetAllLocations
{
    public class GetAllLocationsHandler : IRequestHandler<GetAllLocationsQuery, List<LocationDTO>>
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public GetAllLocationsHandler(ILocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public async Task<List<LocationDTO>> Handle(GetAllLocationsQuery request, CancellationToken cancellationToken)
        {
            var locations = await _locationRepository.GetAllAsync();
            return _mapper.Map<List<LocationDTO>>(locations);
        }
    }
}
