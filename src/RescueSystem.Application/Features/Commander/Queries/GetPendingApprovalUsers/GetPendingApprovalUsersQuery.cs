using MediatR;
using RescueSystem.Application.DTOs.Commander;
using RescueSystem.Application.DTOs.User;

namespace RescueSystem.Application.Features.Commander.Queries.GetPendingApprovalUsers
{
    public class GetPendingApprovalUsersQuery : IRequest<List<UserSystemDTO>>
    {

    }
}