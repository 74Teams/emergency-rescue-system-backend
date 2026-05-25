using RescueSystem.Application.DTOs.Report;

namespace RescueSystem.Application.Common.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task<List<ActivityChartItemDTO>> GetActivityChartAsync(DateTime startDateUtc, int days);
    }
}
