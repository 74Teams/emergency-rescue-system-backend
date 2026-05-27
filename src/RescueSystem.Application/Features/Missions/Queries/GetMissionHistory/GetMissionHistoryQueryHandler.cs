using AutoMapper;
using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Mission;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.Missions.Queries.GetMissionHistory
{
    public class GetMissionHistoryQueryHandler : IRequestHandler<GetMissionHistoryQuery, IEnumerable<MissionHistoryDTO>>
    {
        private readonly IMissionRepository _missionRepository;
        private readonly IMapper _mapper;

        public GetMissionHistoryQueryHandler(IMissionRepository missionRepository, IMapper mapper)
        {
            _missionRepository = missionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MissionHistoryDTO>> Handle(GetMissionHistoryQuery request, CancellationToken cancellationToken)
        {
            var mission = await _missionRepository.GetByIdAsync(request.MissionId);
            if (mission == null)
            {
                throw new NotFoundException("Không tìm thấy nhiệm vụ đối với Id này");
            }

            var histories = await _missionRepository.GetHistoriesByMissionIdAsync(request.MissionId);

            return _mapper.Map<IEnumerable<MissionHistoryDTO>>(histories);
        }
    }
}