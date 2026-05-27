using MediatR;
using Microsoft.AspNetCore.Http;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using System;
using System.Security.Claims;

namespace RescueSystem.Application.Features.Auth.Commands.UpdateProfile
{
    public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateProfileHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception("Unauthorized");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;

            await _userRepository.UpdateUserAsync(user);

            if (request.Address != null)
            {
                var address = new Address
                {
                    UserId = user.Id,
                    Street = request.Address.Street,
                    City = request.Address.City,
                    District = request.Address.District,
                    GPS = request.Address.GPS,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.UpsertAddressAsync(address);
            }

            return "Update profile successfully";
        }
    }
}
