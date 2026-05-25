using MediatR;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Report;

namespace RescueSystem.Application.Features.Report.Queries.GetActivityChart
{
    public class GetActivityChartHandler : IRequestHandler<GetActivityChartQuery, List<ActivityChartItemDTO>>
    {
        private readonly IReportRepository _reportRepository;

        public GetActivityChartHandler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public Task<List<ActivityChartItemDTO>> Handle(GetActivityChartQuery request, CancellationToken cancellationToken)
        {
            var todayUtc = DateTime.UtcNow.Date;
            var startDateUtc = todayUtc.AddDays(-(request.Days - 1));
            return _reportRepository.GetActivityChartAsync(startDateUtc, request.Days);
        }
    }
}
