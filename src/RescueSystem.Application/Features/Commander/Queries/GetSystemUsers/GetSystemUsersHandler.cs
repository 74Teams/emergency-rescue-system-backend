using MediatR;
using RescueSystem.Application.DTOs.Commander;
using RescueSystem.Application.DTOs.User;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.User.Queries.GetSystemUsers
{
    public class GetSystemUsersHandler : IRequestHandler<GetSystemUsersQuery, List<UserSystemDTO>>
    {
        private readonly IUserRepository _userRepository;

        public GetSystemUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserSystemDTO>> Handle(GetSystemUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetSystemUsersAsync(request.Search, request.Role);
            return users.ToList();
        }
    }
}