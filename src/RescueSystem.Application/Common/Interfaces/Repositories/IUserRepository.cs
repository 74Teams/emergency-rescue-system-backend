using RescueSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using RescueSystem.Application.DTOs.User;
using RescueSystem.Application.DTOs.Commander;
using RescueSystem.Application.DTOs.Common;

namespace RescueSystem.Application.Interfaces.Respositories
{
    public interface IUserRepository
    {
        // Create User Methods
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, IList<string> roles);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user);

        // Read/Get User Methods
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<ApplicationUser?> GetUserProfileByIdAsync(Guid userId);
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<PagedResult<UserDTO>> GetPagedUsersAsync(int page, int pageSize, string? search, string? role);

        // Password & Email Methods
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);
        Task<bool> IsEmailConfirmedAsync(ApplicationUser user);

        // Update User Method
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<Address?> GetAddressByUserIdAsync(Guid userId);
        Task UpsertAddressAsync(Address address);

        // Delete User Method
        Task<IdentityResult> DeleteUserAsync(ApplicationUser user);

        // Role Methods
        Task<List<string>> GetUserRolesAsync(ApplicationUser user);

        // Update password for user
        Task UpdatePasswordAsync(ApplicationUser user, string newPassword);

        Task UpdateUserRolesAsync(Guid userId, IList<string> newRoles);

        Task<IList<string>> GetUserRolesAsync(Guid userId);

        Task<IList<UserSystemDTO>> GetPendingApprovalUsers();

        Task<IList<UserSystemDTO>> GetRejectedUsers();
        Task<IList<UserSystemDTO>> GetSystemUsersAsync(string? search, string? role);
    }
}
