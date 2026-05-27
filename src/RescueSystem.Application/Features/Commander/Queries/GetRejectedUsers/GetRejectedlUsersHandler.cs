using MediatR;
using RescueSystem.Application.DTOs.Commander;
using RescueSystem.Application.DTOs.User;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;

namespace RescueSystem.Application.Features.Commander.Queries.GetRejectedUsers
{
    public class GetRejectedUsersHandler : IRequestHandler<GetRejectedUsersQuery, List<UserSystemDTO>>
    {
        private readonly IUserRepository _userRepository;
        public GetRejectedUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<UserSystemDTO>> Handle(GetRejectedUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetRejectedUsers();
            return users.ToList();
        }
    }
}