using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using RescueSystem.Application.DTOs.Common;
using RescueSystem.Application.DTOs.User;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;

namespace RescueSystem.Application.Features.User.Queries.GetAllUser
{
    public class GetAllUserHandler : IRequestHandler<GetAllUserQuery, PagedResult<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        public GetAllUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<PagedResult<UserDTO>> Handle(GetAllUserQuery req, CancellationToken cancellationToken)
        {
            return await _userRepository.GetPagedUsersAsync(
                req.Page,
                req.PageSize,
                req.Search,
                req.Role);
        }
    }
}
