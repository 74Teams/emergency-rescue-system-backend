using MediatR;
using RescueSystem.Application.DTOs.Report;

namespace RescueSystem.Application.Features.Report.Queries.GetActivityChart
{
    public class GetActivityChartQuery : IRequest<List<ActivityChartItemDTO>>
    {
        public int Days { get; set; } = 7;
    }
}
