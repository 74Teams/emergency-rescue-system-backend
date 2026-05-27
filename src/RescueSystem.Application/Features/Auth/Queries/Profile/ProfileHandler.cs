using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Address;
using RescueSystem.Application.DTOs.Auth;
using RescueSystem.Application.DTOs.Contact;

namespace RescueSystem.Application.Features.Auth.Queries.Profile
{
    public class ProfileHandler : IRequestHandler<ProfileQuery, ProfileResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IContactRepository _contactRepository;

        public ProfileHandler(IUserRepository userRepository, IRescueTeamRepository rescueTeamRepository, IContactRepository contactRepository)
        {
            _userRepository = userRepository;
            _rescueTeamRepository = rescueTeamRepository;
            _contactRepository = contactRepository;
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
            var contacts = await _contactRepository.GetByUserIdAsync(foundUser.Id);
            return new ProfileResponse
            {
                Id = foundUser.Id,
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
                Contacts = contacts.Select(contact => new ContactDTO
                {
                    Id = contact.Id,
                    Name = contact.Name,
                    Relationship = contact.Relationship,
                    PhoneNumber = contact.PhoneNumber,
                    Email = contact.Email
                }).ToList(),
                Roles = roles
            };
        }
    }
}
