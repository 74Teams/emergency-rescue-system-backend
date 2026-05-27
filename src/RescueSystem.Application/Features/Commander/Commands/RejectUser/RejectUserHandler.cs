using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.Common.Exception;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.Commander.Commands.RejectUser
{
    public class RejectUserHandler : IRequestHandler<RejectUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public RejectUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(RejectUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId.ToString());

            if (user == null)
                throw new NotFoundException("User not found");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsPendingApproval = false;
            await _userRepository.UpdateUserAsync(user);

            return true;
        }
    }
}