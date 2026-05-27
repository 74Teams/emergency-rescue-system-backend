using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Role.Queries.GetAllRoles
{
    public class GetAllRoleHandler : IRequestHandler<GetAllRoleQuery, List<RoleDTO>>
    {
        private readonly IRoleRepository _roleRepository;
        public GetAllRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<RoleDTO>> Handle(GetAllRoleQuery req, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAllAsync();

            return roles.Select(r => new RoleDTO
            {
                Id = r.Id,
                Name = r.Name!,
                Description = r.Description
            }).ToList();
        }
    }
}
