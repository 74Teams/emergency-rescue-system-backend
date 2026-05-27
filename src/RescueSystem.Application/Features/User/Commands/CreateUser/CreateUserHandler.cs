using MediatR;
using RescueSystem.Application.Common.Response;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.User.Commands
{
    public class CreateUserHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, Unit>
    {

        public async Task<Unit> Handle(CreateUserCommand req, CancellationToken cancellationToken)
        {
            if (req.Roles == null || !req.Roles.Any())
            {
                req.Roles = new List<string> { "Citizen" };
            }
            bool isCitizenOnly = req.Roles.Count == 1 && req.Roles.Contains("Citizen", StringComparer.OrdinalIgnoreCase);

            bool isActive = isCitizenOnly;
            bool isPending = !isCitizenOnly;

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = req.UserName,
                Email = req.Email,
                FullName = req.FullName,
                PhoneNumber = req.PhoneNumber,
                Address = req.Address,
                DateOfBirth = req.DateOfBirth,
                Avatar = req.Avatar,

                IsActive = true,
                IsPendingApproval = isPending,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var result = await userRepository.CreateUserAsync(user, req.Password, req.Roles);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            return Unit.Value;
        }
    }
}
