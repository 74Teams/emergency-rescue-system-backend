using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.Common.Exception;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.Commander.Commands.ToggleUserStatus
{
    public class ToggleUserStatusHandler : IRequestHandler<ToggleUserStatusCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public ToggleUserStatusHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId.ToString());

            if (user == null)
                throw new NotFoundException("User not found");

            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user);

            return true;
        }
    }
}