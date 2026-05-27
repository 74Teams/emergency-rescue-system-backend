using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Request;
using static RescueSystem.Application.DTOs.Request.RequestDTO;

namespace RescueSystem.Application.Features.Request.Queries.GetRequestById
{
    public class GetRequestByIdHandler : IRequestHandler<GetRequestByIdQuery, RequestDTO>
    {
        private readonly IRequestRespository _requestRespository;
        private readonly IMapper _mapper;
        public GetRequestByIdHandler(IRequestRespository requestRespository, IMapper mapper)
        {
            _requestRespository = requestRespository;
            _mapper = mapper;
        }

        public async Task<RequestDTO> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var requestEntity = await _requestRespository.GetByIdAsync(request.RequestId);
            if (requestEntity == null)
            {
                throw new Exception("Không tìm thấy yêu cầu cứu hộ");
            }

            var result = _mapper.Map<RequestDTO>(requestEntity);
            result.Missions = requestEntity.Missions.Select(m => new MissionBriefDto
            {
                Id = m.Id,
                Status = requestEntity.Status,
                RescueTeamId = m.RescueTeamId,
                TeamName = m.RescueTeam?.TeamName,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
            }).ToList();
            return result;
        }
    }
}
