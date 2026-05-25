using MediatR;
using RescueSystem.Application.DTOs.Commander;
using RescueSystem.Application.DTOs.User;
using RescueSystem.Application.Interfaces.Respositories;

namespace RescueSystem.Application.Features.Commander.Queries.GetPendingApprovalUsers
{
    public class GetPendingApprovalUsersHandler : IRequestHandler<GetPendingApprovalUsersQuery, List<UserSystemDTO>>
    {
        private readonly IUserRepository _userRepository;
        public GetPendingApprovalUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<UserSystemDTO>> Handle(GetPendingApprovalUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetPendingApprovalUsers();
            return users.ToList();
        }
    }
}

