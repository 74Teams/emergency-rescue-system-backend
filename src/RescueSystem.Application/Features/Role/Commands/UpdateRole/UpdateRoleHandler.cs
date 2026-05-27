using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Role.Commands.UpdateRole
{
    public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly IRoleRepository _roleRepository;

        public UpdateRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<bool> Handle(UpdateRoleCommand req, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(req.Id);

            if (role == null)
            {
                throw new Exception("Role not found");
            }

            var existing = await _roleRepository.GetByNameAsync(req.Name);
            if (existing != null && existing.Id != req.Id)
            {
                throw new Exception("Role name already exists");
            }

            //Update field
            role.Name = req.Name;
            role.NormalizedName = req.Name.ToUpper();
            role.Description = req.Description;
            role.UpdatedAt = DateTime.UtcNow;

            return await _roleRepository.UpdateAsync(role);
        }
    }
}
