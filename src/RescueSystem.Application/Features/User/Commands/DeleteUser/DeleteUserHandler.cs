using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;

namespace RescueSystem.Application.Features.User.Commands.DeleteUser
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserProfileByIdAsync(request.Id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            await _userRepository.DeleteUserAsync(user);

            return true;
        }
    }
}
