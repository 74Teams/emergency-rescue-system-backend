using RescueSystem.Application.DTOs.Address;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.DTOs.Auth
{
    public class ProfileResponse
    {
        public Guid Id { get; set; }
        public Guid? RescueTeamId { get; set; }
        public string? TeamName { get; set; }
        //alias
        [System.Text.Json.Serialization.JsonPropertyName("fullName")]
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public AddressDTO? Address { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
