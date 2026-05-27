using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.Common.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.User.Commands.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        public UpdateUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserProfileByIdAsync(request.Id);

            if (user == null)
                throw new NotFoundException("User not found");

            user.FullName = request.FullName ?? user.FullName;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.Address = request.Address ?? user.Address;
            user.DateOfBirth = request.DateOfBirth ?? user.DateOfBirth;
            user.Avatar = request.Avatar ?? user.Avatar;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user);

            if (request.Roles != null)
            {
                await _userRepository.UpdateUserRolesAsync(user.Id, request.Roles);
            }

            return true;
        }
    }
}