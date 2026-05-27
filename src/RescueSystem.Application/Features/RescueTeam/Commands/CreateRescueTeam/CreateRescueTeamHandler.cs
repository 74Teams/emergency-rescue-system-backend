using MediatR;
using Microsoft.AspNetCore.Http;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Application.Features.RescueTeam.Commands.CreateRescueTeam;
using RescueSystem.Domain.Enums;
using RescueSystem.Domain.Entities;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System.Linq;

namespace RescueSystem.Application.Features.RescueTeam.Commands.CreateRescueTeam
{
    public class CreateRescueTeamHandler : IRequestHandler<CreateRescueTeamCommand, RescueTeamDTO>
    {
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IUserRepository _userRepository;


        public CreateRescueTeamHandler(IRescueTeamRepository rescueTeamRepository, IUserRepository userRepository)
        {
            _rescueTeamRepository = rescueTeamRepository;
            _userRepository = userRepository;
        }

        public async Task<RescueTeamDTO> Handle(CreateRescueTeamCommand request, CancellationToken cancellationToken)
        {
            var teamId = Guid.NewGuid();
            var rescueTeam = new RescueSystem.Domain.Entities.RescueTeam
            {
                Id = teamId,
                TeamName = request.TeamName,
                Description = request.Description,
                TeamLeaderId = request.TeamLeaderId,
                BaseLocationId = request.BaseLocationId,
                Status = TeamStatus.AVAILABLE
            };

            var createdTeam = await _rescueTeamRepository.CreateAsync(rescueTeam);
            if (!createdTeam)
            {
                throw new Exception("Không thể tạo đội cứu hộ");
            }

            var currentRoles = await _userRepository.GetUserRolesAsync(request.TeamLeaderId);
            var rolesList = currentRoles.ToList();
            if (!rolesList.Contains("RescuerLeader"))
            {
                rolesList.Add("RescuerLeader");
            }
            else
            {
                throw new Exception("Người này đã là leader của một đội cứu hộ khác");
            }
            var memberAdded = await _rescueTeamRepository.AddMemberAsync(teamId, request.TeamLeaderId);
            if (!memberAdded)
            {
                throw new Exception("Tạo đội thành công nhưng không thể thêm Đội trưởng vào danh sách thành viên.");
            }
            return new RescueTeamDTO
            {
                Id = rescueTeam.Id,
                TeamName = rescueTeam.TeamName,
                Status = rescueTeam.Status.ToString()
            };
        }
    }
}