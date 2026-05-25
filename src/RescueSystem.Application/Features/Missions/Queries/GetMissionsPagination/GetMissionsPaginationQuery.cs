using MediatR;
using RescueSystem.Application.Common.Models;
using RescueSystem.Application.DTOs.Mission;
using RescueSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Missions.Queries.GetMissionsPagination
{

    public class GetMissionsPaginationQuery : IRequest<PagedResult<MissionDTO>>
    {
        public int Page { get; set; } = 1; // trang hiện tại với mặc định là 1
        public int PageSize { get; set; } = 10; // số records trên mỗi trang với mặc định là 10

        public MissionStatus? Status { get; set; } // tùy chọn lọc theo trạng thái nhiệm vụ
        public Guid? RescueTeamId { get; set; } // tùy chọn lọc theo đội cứu hộ
    }
}
