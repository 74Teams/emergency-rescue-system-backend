using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RescueSystem.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public RoleRepository(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<List<ApplicationRole>> GetAllAsync()
        {
            return await _roleManager.Roles.AsNoTracking().ToListAsync();
        }

        public async Task<ApplicationRole?> GetByIdAsync(Guid id)
        {
            return await _roleManager.FindByIdAsync(id.ToString());
        }

        public async Task<ApplicationRole?> GetByNameAsync(string name)
        {
            return await _roleManager.FindByNameAsync(name);
        }

        public async Task<bool> CreateAsync(ApplicationRole role)
        {
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            return true;
        }

        public async Task<bool> UpdateAsync(ApplicationRole role)
        {
            role.UpdatedAt = DateTime.UtcNow;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(ApplicationRole role)
        {
            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            return true;
        }
    }
}
