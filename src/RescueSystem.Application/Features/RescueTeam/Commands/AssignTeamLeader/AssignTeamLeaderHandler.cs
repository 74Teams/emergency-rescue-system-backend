using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RescueSystem.Application.Common.Response;
using RescueSystem.Domain.Entities;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Application.Interfaces.Respositories;
using RescueSystem.Application.Common.Exception;

namespace RescueSystem.Application.Features.RescueTeam.Commands.AssignTeamLeader
{
    public class AssignTeamLeaderHandler : IRequestHandler<AssignTeamLeaderCommand, ApiResponse<object>>
    {
        private readonly IRescueTeamRepository _rescueTeamRepository;
        private readonly IUserRepository _userRepository; 
        public AssignTeamLeaderHandler(
            IRescueTeamRepository rescueTeamRepository, 
            IUserRepository userRepository) 
        {
            _rescueTeamRepository = rescueTeamRepository;
            _userRepository = userRepository;
        }
        public async Task<ApiResponse<object>> Handle(AssignTeamLeaderCommand request, CancellationToken cancellationToken)
        {
            var team = await _rescueTeamRepository.GetByIdAsync(request.TeamId);
            if(team == null) return ApiResponse<object>.ErrorResponse("Không tìm thấy đội cứu hộ", StatusCodes.Status404NotFound);            

            var user = await _userRepository.GetUserByIdAsync(request.UserId.ToString());
            if(user == null) return ApiResponse<object>.ErrorResponse("Không tìm thấy thông tin nhân sự", StatusCodes.Status404NotFound);    
            if (!user.IsActive) return ApiResponse<object>.ErrorResponse("Không thể bổ nhiệm nhân sự đang bị khóa.", StatusCodes.Status400BadRequest);            

            var currentRoles = await _userRepository.GetUserRolesAsync(request.UserId);
            if(currentRoles.Contains("RescuerLeader")) return ApiResponse<object>.ErrorResponse("Không thể bổ nhiệm người này vì họ đã là đội trưởng của một đội.");            


            if (user.RescueTeamId.HasValue && user.RescueTeamId != request.TeamId)
            {
                var isRemoved = await _rescueTeamRepository.RemoveMemberAsync(user.RescueTeamId.Value, user.Id);
                if (!isRemoved) return ApiResponse<object>.ErrorResponse("Lỗi hệ thống khi rút nhân sự khỏi đội cũ.", StatusCodes.Status500InternalServerError);
                
                await _rescueTeamRepository.AddMemberAsync(request.TeamId, user.Id);
            }
            else if (!user.RescueTeamId.HasValue)
            {
                await _rescueTeamRepository.AddMemberAsync(request.TeamId, user.Id);
            }

            if (currentRoles.Contains("Rescuer")) currentRoles.Remove("Rescuer");
            currentRoles.Add("RescuerLeader");
            await _userRepository.UpdateUserRolesAsync(request.UserId, currentRoles);

            team.TeamLeaderId = request.UserId;
            var isTeamUpdated = await _rescueTeamRepository.UpdateRescueTeamAsync(team);
            if (!isTeamUpdated) return ApiResponse<object>.ErrorResponse("Lỗi hệ thống khi gán chức vụ đội trưởng.", StatusCodes.Status500InternalServerError);
            return ApiResponse<object>.SuccessResponse(null, $"Bổ nhiệm sĩ quan thành công!", StatusCodes.Status200OK);
        }
    }
}