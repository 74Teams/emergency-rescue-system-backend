using System;
using System.Collections.Generic;
using System.Text;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Application.DTOs.User;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Application.DTOs.Mission
{

    //EDIT:DIEU 18/05/2026 Chuẩn hóa lại DTO cho Mision
    public class MissionDTO
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public string? Description { get; set; }

        // FIXED: Use nested UserDTO instead of flat properties
        public UserDTO? Dispatcher { get; set; }

        // FIXED: Use nested RescueTeamDTO instead of flat properties
        public RescueTeamDTO? RescueTeam { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

        // FIXED: Use enum instead of string
        public MissionStatus Status { get; set; }
    }
}

//Note: Sao lại để mấy cái name name ở đây hè, có thể lấy từ db rồi map vào chứ để thế này hơi k ổn, nếu để thế này thì phải đảm bảo lúc tạo mission phải lấy tên dispatcher và team rồi gán vào, nếu sau này có đổi tên thì lại phải update lại mấy cái mission liên quan nữa, nên tốt nhất là bỏ mấy cái name đi chỉ giữ id thôi, còn muốn lấy tên thì join với bảng user và rescue team rồi map vào DTO là được