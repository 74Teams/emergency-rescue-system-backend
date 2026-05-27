using AutoMapper;
using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Location;

namespace RescueSystem.Application.Features.Location.Queries.GetLocationById
{
    public class GetLocationByIdHandler : IRequestHandler<GetLocationByIdQuery, LocationDTO>
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public GetLocationByIdHandler(ILocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public async Task<LocationDTO> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
        {
            var location = await _locationRepository.GetByIdAsync(request.Id);
            if (location == null)
            {
                throw new NotFoundException("Không tìm thấy vị trí");
            }

            return _mapper.Map<LocationDTO>(location);
        }
    }
}
