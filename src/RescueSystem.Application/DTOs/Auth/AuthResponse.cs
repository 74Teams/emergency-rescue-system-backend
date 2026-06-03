using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public int ExpiresIn { get; set; } = 3600;
        public AuthUserDTO User { get; set; } = new();
    }

    public class AuthUserDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public string Avatar { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsPendingApproval { get; set; }
    }
}
