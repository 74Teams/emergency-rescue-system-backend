using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Common;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Infrastructure.Persistence.Repositories
{
    public class RequestRepository : IRequestRespository
    {
        private readonly ApplicationDbContext _context;
        public RequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<RescueRequest>> GetAllAsync()
        {
            return _context.Requests
                .AsNoTracking()
                .ToListAsync();
        }
        public Task<RescueRequest?> GetByIdAsync(Guid id)
        {

            return _context.Requests
                .AsNoTracking()
                .Include(r => r.Medias)
                .Include(r => r.RequestedBy)
                .Include(r => r.Location)
                .Include(r => r.Missions)
                    .ThenInclude(m => m.RescueTeam)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        public Task<List<RescueRequest>> GetByUserIdAsync(Guid userId)
        {
            return _context.Requests
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
        public Task<List<RescueRequest>> GetByStatusAsync(RequestStatus status)
        {
            return _context.Requests
                .AsNoTracking()
                .Where(r => r.Status == status)
                .ToListAsync();
        }

        public async Task<PagedResult<RescueRequest>> GetPagedAsync(int page, int pageSize, RequestStatus? status = null, Priority? priority = null, EmergencyType? emergencyType = null, string? sortBy = null)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var query = _context.Requests
                .AsNoTracking()
                .Include(r => r.Medias)
                .Include(r => r.RequestedBy)
                .Include(r => r.Location)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);

            if (priority.HasValue)
                query = query.Where(r => r.Priority == priority.Value);

            if (emergencyType.HasValue)
                query = query.Where(r => r.EmergencyType == emergencyType.Value);

            query = sortBy?.ToLower() switch
            {
                "priority" => query.OrderBy(r => r.Priority),
                "createdat" => query.OrderBy(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<RescueRequest>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        public async Task CreateAsync(RescueRequest request)
        {
            await _context.Requests.AddAsync(request);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(RescueRequest request)
        {
            _context.Entry(request).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task UpdateStatusAsync(Guid requestId, RequestStatus status)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (request != null)
            {
                request.Status = status;
                await _context.SaveChangesAsync();
                return;
            }
            throw new BadRequestException("Yêu cầu cứu hộ không hợp lệ");

        }
        public async Task DeleteAsync(RescueRequest request)
        {
            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();
        }
    }
}
