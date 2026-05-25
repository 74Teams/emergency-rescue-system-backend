using MediatR;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Application.Features.User.Commands;
using RescueSystem.Application.Interfaces.Respositories;
using RescueSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Role.Commands.CreateRole
{
    public class CreateRoleHandler(IRoleRepository roleRepository) : IRequestHandler<CreateRoleCommand, Unit>
    {

        public async Task<Unit> Handle(CreateRoleCommand req, CancellationToken cancellationToken)
        {

            var role = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = req.RoleName,
                Description = req.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await roleRepository.CreateAsync(role);

            return Unit.Value;
        }
    }
}
