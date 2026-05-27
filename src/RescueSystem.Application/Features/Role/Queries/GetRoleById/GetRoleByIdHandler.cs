using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Role;
using RescueSystem.Application.DTOs.User;
using RescueSystem.Application.Features.User.Queries.GetUserById;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Role.Queries.GetAllRoles
{
    public class GetRoleByIdHandler(IRoleRepository roleRepository) : IRequestHandler<GetRoleByIdQuery, RoleDTO>
    {
        public async Task<RoleDTO> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await roleRepository.GetByIdAsync(request.Id);

            if (role == null)
            {
                throw new Exception("Role not found");

            }

            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name!,
                Description = role.Description
            };
        }
    }
}
