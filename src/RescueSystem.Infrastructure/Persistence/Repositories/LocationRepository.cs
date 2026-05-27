using Microsoft.EntityFrameworkCore;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Infrastructure.Persistence.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _context;

        public LocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Location>> GetAllAsync()
        {
            return await _context.Locations
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Location?> GetByIdAsync(Guid id)
        {
            return await _context.Locations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> CreateAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Location location)
        {
            _context.Locations.Update(location);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Location location)
        {
            _context.Locations.Remove(location);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
