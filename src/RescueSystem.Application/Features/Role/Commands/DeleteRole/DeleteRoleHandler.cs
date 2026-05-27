using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Role.Commands.DeleteRole
{
    public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly IRoleRepository _roleRepository;

        public DeleteRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<bool> Handle(DeleteRoleCommand req, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(req.Id);

            if (role == null)
            {
                throw new Exception("Role not found");
            }

            return await _roleRepository.DeleteAsync(role);
        }
    }
}
