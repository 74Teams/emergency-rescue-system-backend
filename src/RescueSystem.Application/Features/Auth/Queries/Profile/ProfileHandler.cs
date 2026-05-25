using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Address;
using RescueSystem.Application.DTOs.Auth;
using RescueSystem.Application.Interfaces.Respositories;

namespace RescueSystem.Application.Features.Auth.Queries.Profile
{
    public class ProfileHandler : IRequestHandler<ProfileQuery, ProfileResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRescueTeamRepository _rescueTeamRepository;

        public ProfileHandler(IUserRepository userRepository, IRescueTeamRepository rescueTeamRepository)
        {
            _userRepository = userRepository;
            _rescueTeamRepository = rescueTeamRepository;
        }
        public async Task<ProfileResponse> Handle(ProfileQuery request, CancellationToken cancellationToken)
        {
            var foundUser = await _userRepository.GetUserByIdAsync(request.UserId);
            if (foundUser == null)
            {
                throw new NotFoundException("User không tồn tại.");
            }

            var roles = await _userRepository.GetUserRolesAsync(foundUser);

            var address = await _userRepository.GetAddressByUserIdAsync(foundUser.Id);

            string? teamName = null;
            if (foundUser.RescueTeamId.HasValue)
            {
                var team = await _rescueTeamRepository.GetByIdAsync(foundUser.RescueTeamId.Value);
                teamName = team?.TeamName;
            }

            return new ProfileResponse
            {
                Id = foundUser.Id,
                RescueTeamId = foundUser.RescueTeamId,
                TeamName = teamName,
                Fullname = foundUser.FullName,
                Email = foundUser.Email ?? string.Empty,
                PhoneNumber = foundUser.PhoneNumber ?? string.Empty,
                Address = address == null ? null : new AddressDTO
                {
                    Street = address.Street,
                    City = address.City,
                    District = address.District,
                    GPS = address.GPS
                },
                Roles = roles
            };
        }
    }
}
