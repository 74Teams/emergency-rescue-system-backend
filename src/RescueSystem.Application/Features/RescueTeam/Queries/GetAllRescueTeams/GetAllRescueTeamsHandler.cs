
using AutoMapper;
using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.RescueTeam;

namespace RescueSystem.Application.Features.RescueTeam.Queries.GetAllRescueTeams
{
    public class GetAllRescueTeamsHandler : IRequestHandler<GetAllRescueTeamsQuery, List<RescueTeamDTO>>
    {

        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IMapper _mapper;
        public GetAllRescueTeamsHandler(IRescueTeamRepository rescueTeamRepository, IMapper mapper)
        {
            _rescueTeamRepository = rescueTeamRepository;
            _mapper = mapper;
        }

        public async Task<List<RescueTeamDTO>> Handle(GetAllRescueTeamsQuery request, CancellationToken cancellationToken)
        {
            var rescueTeams = await _rescueTeamRepository.GetAllAsync();
            return _mapper.Map<List<RescueTeamDTO>>(rescueTeams);
        }
    }
}