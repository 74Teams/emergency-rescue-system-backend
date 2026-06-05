using MediatR;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Dispatcher;
using RescueSystem.Application.DTOs.Location;
using RescueSystem.Application.DTOs.Mission;
using RescueSystem.Application.DTOs.Request;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Application.DTOs.User;
using RescueSystem.Application.Features.Checklist;
using RescueSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.Missions.Queries.GetMissionById
{
    public class GetMissionByIdHandler(IMissionRepository missionRepository) : IRequestHandler<GetMissionByIdQuery, MissionDetailDTO>
    {
        public async Task<MissionDetailDTO> Handle(GetMissionByIdQuery request, CancellationToken cancellationToken)
        {
            var mission = await missionRepository.GetByIdAsync(request.Id);
            if (mission == null)
            {
                throw new NotFoundException("Không tìm thấy nhiệm vụ");
            }

            return new MissionDetailDTO
            {
                Id = mission.Id,

                Request = mission.Request == null ? null : new RequestDTO
                {
                    Id = mission.Request.Id,
                    Description = mission.Request.Description,
                    EmergencyType = mission.Request.EmergencyType,
                    Priority = mission.Request.Priority,
                    Status = mission.Request.Status,
                    Location = mission.Request.Location != null ? new LocationDTO
                    {
                        Id = mission.Request.Location.Id,
                        Latitude = mission.Request.Location.Latitude,
                        Longitude = mission.Request.Location.Longitude,
                        Address = mission.Request.Location.Address,
                        Landmark = mission.Request.Location.Landmark
                    } : null, // quan trọng
                    RequestedBy = mission.Request.RequestedBy != null ? new UserDTO
                    {
                        Id = mission.Request.RequestedBy.Id,
                        FullName = mission.Request.RequestedBy.FullName,
                        Email = mission.Request.RequestedBy.Email,
                        UserName = mission.Request.RequestedBy.UserName,
                        PhoneNumber = mission.Request.RequestedBy.PhoneNumber,
                        Address = mission.Request.RequestedBy.Address,
                        DateOfBirth = mission.Request.RequestedBy.DateOfBirth,
                        Avatar = mission.Request.RequestedBy.Avatar ?? string.Empty
                    } : null,
                    Medias = mission.Request.Medias?.Select(m => new RequestMediaDTO
                    {
                        Id = m.Id,
                        SecureUrl = m.SecureUrl,
                        PublicId = m.PublicId,
                        ResourceType = m.ResourceType
                    }).ToList() ?? new List<RequestMediaDTO>(),
                },


                RescueTeam = mission.RescueTeam == null ? null : new RescueTeamDTO
                {
                    Id = mission.RescueTeam.Id,
                    TeamName = mission.RescueTeam.TeamName,
                    Status = mission.RescueTeam.Status.ToString()
                },

                Dispatcher = mission.Dispatcher == null ? null : new DispatcherDTO
                {
                    Id = mission.Dispatcher.Id,
                    Name = mission.Dispatcher.FullName,
                    Email = mission.Dispatcher.Email
                },
                Status = mission.Request!.Status,
                StartTime = mission.StartTime.AddHours(7),
                EndTime = mission.EndTime.HasValue ? mission.EndTime.Value.AddHours(7) : null,
                CreateAt = mission.CreatedAt.AddHours(7),
                UpdateAt = mission.UpdatedAt.AddHours(7),
                Checklists = mission.Checklists?.Select(c => new ChecklistDetailDTO
                {
                    Id = c.Id,
                    Title = c.Title,
                    MissionId = c.MissionId,
                    CreatedAt = c.CreatedAt.AddHours(7),
                    UpdatedAt = c.UpdatedAt.AddHours(7),
                    Items = c.ChecklistItems?.Select(i => new ChecklistItemDTO
                    {
                        Id = i.Id,
                        Description = i.Description,
                        IsCheck = i.IsCheck,
                        CreatedAt = i.CreatedAt.AddHours(7),
                        UpdatedAt = i.UpdatedAt.AddHours(7)
                    }).ToList() ?? new List<ChecklistItemDTO>()
                }).ToList() ?? new List<ChecklistDetailDTO>()
            };
        }
    }
}
