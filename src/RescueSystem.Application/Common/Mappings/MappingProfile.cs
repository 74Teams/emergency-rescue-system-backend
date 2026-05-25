using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using RescueSystem.Application.DTOs.Location;
using RescueSystem.Application.DTOs.Request;
using RescueSystem.Application.DTOs.User;
using RescueSystem.Application.DTOs.Mission;
using RescueSystem.Domain.Entities;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Application.DTOs.Commander;

namespace RescueSystem.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapping for Request entity
        CreateMap<ApplicationUser, UserDTO>();
        CreateMap<RequestMedia, RequestMediaDTO>();
        CreateMap<RescueRequest, RequestDTO>();
        CreateMap<RescueRequest, NonRelationRequestDTO>();
        CreateMap<Location, LocationDTO>();

        // Mission mappings
        CreateMap<MissionHistory, MissionHistoryDTO>();

        CreateMap<Mission, RequestDTO.MissionBriefDto>()
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.RescueTeam != null ? src.RescueTeam.TeamName : string.Empty));

        CreateMap<RescueTeam, RescueTeamDTO>()
            .ForMember(dest => dest.Leader, opt => opt.MapFrom(src => src.TeamLeader))
            .ForMember(dest => dest.LeaderId, opt => opt.MapFrom(src => src.TeamLeaderId))
            .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.Members != null ? src.Members.Count : 0));
        CreateMap<ApplicationUser, UserSystemDTO>()
            .IncludeBase<ApplicationUser, UserDTO>()
            .ForMember(dest => dest.IsPendingApproval, opt => opt.MapFrom(src => src.IsPendingApproval));
    }
}
