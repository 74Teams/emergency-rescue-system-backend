using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.DTOs.Commander;
using RescueSystem.Application.DTOs.Common;
using RescueSystem.Application.DTOs.User;
using RescueSystem.Application.Interfaces.Respositories;
using RescueSystem.Domain.Entities;
using RescueSystem.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Create User with password
        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }
        // Create User with password and roles
        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, IList<string> roles)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded && roles != null)
            {
                await _userManager.AddToRolesAsync(user, roles);
            }
            return result;
        }
        // Create User without password
        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user)
        {
            return await _userManager.CreateAsync(user);
        }

        // check password
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        // Find user by email
        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        // Find user by id
        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        // Confirm email
        public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result;
        }

        // Update user
        public async Task UpdateUserAsync(ApplicationUser user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id.ToString());

            if (existingUser == null)
            {
                throw new Exception("User not found");
            }

            existingUser.FullName = user.FullName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Address = user.Address;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.Avatar = user.Avatar;

            var result = await _userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to update user");
            }
        }


        public async Task<Address?> GetAddressByUserIdAsync(Guid userId)
        {
            return await _context.Addresses.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public async Task UpsertAddressAsync(Address address)
        {
            var existing = await _context.Addresses.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == address.UserId);
            if (existing == null)
            {
                await _context.Addresses.AddAsync(address);
            }
            else
            {
                existing.Street = address.Street;
                existing.City = address.City;
                existing.District = address.District;
                existing.GPS = address.GPS;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        // Delete user
        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }
        // get all users
        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<PagedResult<UserDTO>> GetPagedUsersAsync(int page, int pageSize, string? search, string? role)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var query = _context.Users
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();
                query = query.Where(u =>
                    u.FullName.Contains(keyword) ||
                    (!string.IsNullOrEmpty(u.UserName) && u.UserName.Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.PhoneNumber) && u.PhoneNumber.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                    _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == role)));
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userIds = users.Select(u => u.Id).ToList();

            var roleLinks = await (
                from userRole in _context.UserRoles
                join roleEntity in _context.Roles on userRole.RoleId equals roleEntity.Id
                where userIds.Contains(userRole.UserId)
                select new { userRole.UserId, RoleName = roleEntity.Name }
            ).ToListAsync();

            var rolesByUser = roleLinks
                .GroupBy(link => link.UserId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(link => link.RoleName)
                        .Where(name => !string.IsNullOrEmpty(name))
                        .Select(name => name!)
                        .ToList());

            var items = users.Select(user => new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Avatar = user.Avatar,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = rolesByUser.TryGetValue(user.Id, out var roles) ? roles : new List<string>()
            }).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedResult<UserDTO>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        // get user by profile id
        public async Task<ApplicationUser?> GetUserProfileByIdAsync(Guid userId)
        {
            return await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        }

        // check isEmailConfirmed
        public async Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }
        // get user roles
        public async Task<List<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        // update password for user
        public async Task UpdatePasswordAsync(ApplicationUser user, string newPassword)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
                throw new Exception("Reset password failed");
        }

        public async Task UpdateUserRolesAsync(Guid userId, IList<string> newRoles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng để cập nhật quyền.");
            }

            if (newRoles == null || !newRoles.Any())
            {
                newRoles = new List<string> { "Citizen" };
            }

            // 2. Lấy danh sách Roles hiện tại trong Database
            var currentRoles = await _userManager.GetRolesAsync(user);

            var rolesToRemove = currentRoles.Except(newRoles).ToList();
            var rolesToAdd = newRoles.Except(currentRoles).ToList();

            if (rolesToRemove.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            if (rolesToAdd.Any())
            {
                await _userManager.AddToRolesAsync(user, rolesToAdd);
            }
        }

        public async Task<IList<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new List<string>();
            }

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IList<UserSystemDTO>> GetPendingApprovalUsers()
        {
            var query = from u in _context.Users
                        where u.IsPendingApproval == true
                        select new UserSystemDTO
                        {
                            Id = u.Id,
                            FullName = u.FullName,
                            Email = u.Email,
                            UserName = u.UserName,
                            PhoneNumber = u.PhoneNumber,
                            Address = u.Address,
                            DateOfBirth = u.DateOfBirth,
                            Avatar = u.Avatar,
                            IsActive = u.IsActive,
                            IsPendingApproval = u.IsPendingApproval,
                            CreatedAt = u.CreatedAt,
                            Roles = (from userRole in _context.UserRoles
                                     join role in _context.Roles on userRole.RoleId equals role.Id
                                     where userRole.UserId == u.Id
                                     select role.Name).ToList()
                        };

            return await query.ToListAsync();
        }
        public async Task<IList<UserSystemDTO>> GetRejectedUsers()
        {
            var query = from u in _context.Users
                        where u.IsPendingApproval == false && u.IsActive == false
                        select new UserSystemDTO
                        {
                            Id = u.Id,
                            FullName = u.FullName,
                            Email = u.Email,
                            UserName = u.UserName,
                            PhoneNumber = u.PhoneNumber,
                            Address = u.Address,
                            DateOfBirth = u.DateOfBirth,
                            Avatar = u.Avatar,
                            IsActive = u.IsActive,
                            IsPendingApproval = u.IsPendingApproval,
                            CreatedAt = u.CreatedAt,
                            Roles = (from userRole in _context.UserRoles
                                     join role in _context.Roles on userRole.RoleId equals role.Id
                                     where userRole.UserId == u.Id
                                     select role.Name).ToList()
                        };

            return await query.ToListAsync();
        }

        public async Task<IList<UserSystemDTO>> GetSystemUsersAsync(string? search, string? role)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.FullName.Contains(search)
                //TODO: Them cai nay vo
                // || u.UserName.Contains(search) 
                // || u.Email.Contains(search)
                );
            }

            // Lọc theo Role
            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id &&
                            _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == role)));
            }

            return await query.Select(u => new UserSystemDTO
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                UserName = u.UserName,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                IsPendingApproval = u.IsPendingApproval,
                CreatedAt = u.CreatedAt,

                Roles = _context.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Select(ur => _context.Roles.FirstOrDefault(r => r.Id == ur.RoleId)!.Name!)
                    .ToList()
            }).ToListAsync();
        }
    }

}
