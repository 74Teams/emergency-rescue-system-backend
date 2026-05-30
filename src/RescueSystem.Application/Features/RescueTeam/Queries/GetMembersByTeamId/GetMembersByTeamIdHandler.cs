using MediatR;
using Microsoft.AspNetCore.Identity;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.RescueTeam.Queries.GetMembersByTeamId
{
    public class GetMembersByTeamIdHandler : IRequestHandler<GetMembersByTeamIdQuery, List<RescueTeamMemberDTO>>
    {
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetMembersByTeamIdHandler(IRescueTeamRepository rescueTeamRepository, UserManager<ApplicationUser> userManager)
        {
            _rescueTeamRepository = rescueTeamRepository;
            _userManager = userManager;
        }

        public async Task<List<RescueTeamMemberDTO>> Handle(GetMembersByTeamIdQuery request, CancellationToken cancellationToken)
        {
            var members = await _rescueTeamRepository.GetMembersByTeamIdAsync(request.TeamId);
            
            var dtoList = new List<RescueTeamMemberDTO>();
            foreach (var m in members)
            {
                var roles = await _userManager.GetRolesAsync(m);
                dtoList.Add(new RescueTeamMemberDTO
                {
                    Id = m.Id,
                    FullName = m.FullName,
                    Email = m.Email ?? string.Empty,
                    PhoneNumber = m.PhoneNumber,
                    Avatar = m.Avatar,
                    IsActive = m.IsActive,
                    Roles = roles.ToList()
                });
            }
            
            return dtoList;
        }
    }
}