using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.DTOs.User;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.User.Queries.GetUserById
{
    public class GetUserByIdHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, UserDTO>
    {


        public async Task<UserDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserProfileByIdAsync(request.Id);

            if (user == null)
            {
                // log warning if needed (logger not injected here)
                throw new NotFoundException("Do not exit");

            }

            var roles = await userRepository.GetUserRolesAsync(user);

            var res = new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Avatar = user.Avatar,
                Roles = roles.ToList()
            };
            return res;
        }
    }
}


