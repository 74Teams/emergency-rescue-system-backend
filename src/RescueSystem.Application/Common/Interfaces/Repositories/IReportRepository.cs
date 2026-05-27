using RescueSystem.Application.DTOs.Report;

namespace RescueSystem.Infrastructure.Common.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task<List<ActivityChartItemDTO>> GetActivityChartAsync(DateTime startDateUtc, int days);
    }
}
