using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.Common.Exception;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.Commander.Commands.ApproveUser
{
    public class ApproveUserHandler : IRequestHandler<ApproveUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public ApproveUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(ApproveUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId.ToString());

            if (user == null)
                throw new NotFoundException("User not found");

            user.IsPendingApproval = false;
            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user);

            return true;
        }
    }
}