using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using RescueSystem.Application.DTOs.Common;
using RescueSystem.Application.DTOs.User;

namespace RescueSystem.Application.Features.User.Queries.GetAllUser
{

    public class GetAllUserQuery : IRequest<PagedResult<UserDTO>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? Role { get; set; }
    }
}
