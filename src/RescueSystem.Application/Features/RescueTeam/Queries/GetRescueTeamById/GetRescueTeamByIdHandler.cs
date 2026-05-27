using AutoMapper;
using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.RescueTeam;

namespace RescueSystem.Application.Features.RescueTeam.Queries.GetRescueTeamById
{
    public class GetRescueTeamByIdHandler : IRequestHandler<GetRescueTeamByIdQuery, RescueTeamDTO>
    {
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IMapper _mapper;

        public GetRescueTeamByIdHandler(IRescueTeamRepository rescueTeamRepository, IMapper mapper)
        {
            _rescueTeamRepository = rescueTeamRepository;
            _mapper = mapper;
        }

        public async Task<RescueTeamDTO> Handle(GetRescueTeamByIdQuery request, CancellationToken cancellationToken)
        {
            var team = await _rescueTeamRepository.GetByIdAsync(request.Id);
            if (team == null)
            {
                throw new NotFoundException("Rescue team not found");
            }
            return _mapper.Map<RescueTeamDTO>(team);
        }
    }
}