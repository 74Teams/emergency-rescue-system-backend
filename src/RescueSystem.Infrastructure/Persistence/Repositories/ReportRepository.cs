using Microsoft.EntityFrameworkCore;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Report;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Infrastructure.Persistence.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ActivityChartItemDTO>> GetActivityChartAsync(DateTime startDateUtc, int days)
        {
            var startDate = startDateUtc.Date;
            var endDate = startDate.AddDays(days);

            var requestsByDay = await _context.Requests
                .AsNoTracking()
                .Where(r => r.CreatedAt >= startDate && r.CreatedAt < endDate)
                .GroupBy(r => r.CreatedAt.Date)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .ToListAsync();

            var resolvedByDay = await _context.Requests
                .AsNoTracking()
                .Where(r => r.Status == RequestStatus.COMPLETED && r.UpdatedAt >= startDate && r.UpdatedAt < endDate)
                .GroupBy(r => r.UpdatedAt.Date)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .ToListAsync();

            var requestsLookup = requestsByDay.ToDictionary(x => x.Day, x => x.Count);
            var resolvedLookup = resolvedByDay.ToDictionary(x => x.Day, x => x.Count);

            var results = new List<ActivityChartItemDTO>(days);
            for (var i = 0; i < days; i++)
            {
                var day = startDate.AddDays(i);
                results.Add(new ActivityChartItemDTO
                {
                    Day = day.ToString("dd/MM"),
                    Requests = requestsLookup.TryGetValue(day, out var requestCount) ? requestCount : 0,
                    Resolved = resolvedLookup.TryGetValue(day, out var resolvedCount) ? resolvedCount : 0
                });
            }

            return results;
        }
    }
}
