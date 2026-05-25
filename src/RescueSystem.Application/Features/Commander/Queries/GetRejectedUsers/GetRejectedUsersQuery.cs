using MediatR;
using RescueSystem.Application.DTOs.Commander;
using RescueSystem.Application.DTOs.User;

namespace RescueSystem.Application.Features.Commander.Queries.GetRejectedUsers
{
    public class GetRejectedUsersQuery : IRequest<List<UserSystemDTO>>
    {

    }
}