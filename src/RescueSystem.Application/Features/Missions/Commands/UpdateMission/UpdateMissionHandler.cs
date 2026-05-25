// using MediatR;
// using RescueSystem.Application.Common.Interfaces.Repositories;
// using System;
// using System.Collections.Generic;
// using System.Text;
// using RescueSystem.Domain.Entities;
// using RescueSystem.Domain.Enums;

// namespace RescueSystem.Application.Features.Missions.Commands.UpdateMission
// {
//     public class UpdateMissionHandler : IRequestHandler<UpdateMissionCommand, bool>
//     {
//         private readonly IMissionRepository _missionRepository;
//         public UpdateMissionHandler(IMissionRepository missionRepository)
//         {
//             _missionRepository = missionRepository;
//         }

//         public async Task<bool> Handle(UpdateMissionCommand request, CancellationToken cancellationToken)
//         {
//             var mission = await _missionRepository.GetByIdAsync(request.MissionId); // laays nhiem vu
//             if (mission == null)
//             {
//                 throw new Exception("Không tìm thấy nhiệm vụ!");
//             } // neu ko tim thay

//              if (request.Status == MissionStatus.ABORTED)
//             {
//                 if (mission.Status == MissionStatus.COMPLETED || mission.Status == MissionStatus.ABORTED)
//                 {
//                     return false;
//                 }
//             }
//              else if (mission.Status == MissionStatus.COMPLETED || mission.Status == MissionStatus.ABORTED)
//             {
//                 // Cannot update completed/aborted missions
//                 return false;
//             }
//              else if (request.Status == MissionStatus.COMPLETED)
//             {
//                 // COMPLETED should only come from FinishMission endpoint
//                 return false;
//             }
//             // k update trang thái kết thúc
//             if (mission.Status == MissionStatus.COMPLETED
//                 || mission.Status == MissionStatus.ABORTED)
//             {
//                 return false;
//             }

//             // k cho yêu cầu chuyển trạng thái này
//             if (request.Status == MissionStatus.COMPLETED
//                 || request.Status == MissionStatus.ABORTED)
//                 return false;

//             // chuyen trang thai
//             bool isValidTransition = false;

//             if (mission.Status == MissionStatus.ASSIGNED
//                 && request.Status == MissionStatus.EN_ROUTE)
//                 isValidTransition = true;

//             else if (mission.Status == MissionStatus.EN_ROUTE
//                 && request.Status == MissionStatus.ON_SITE)
//                 isValidTransition = true;

//             else if (mission.Status == MissionStatus.ON_SITE
//                 && request.Status == MissionStatus.IN_PROGRESS)
//                 isValidTransition = true;

//             if (!isValidTransition)
//             {
//                 return false; // chuyen trang thai khong hop le
//             }

//             var previousStatus = mission.Status;

//             // cap nhat cais trang thai
//             mission.Status = request.Status;

//             // thoi gian update
//             mission.UpdatedAt = DateTime.UtcNow.AddHours(7);

//             var history = new MissionHistory
//             {
//                 MissionId = mission.Id,
//                 FromStatus = previousStatus,
//                 ToStatus = mission.Status,
//                 ChangedById = request.ChangedById,
//                 Note = request.Note,
//                 CreatedAt = DateTime.UtcNow.AddHours(7)
//             };

//             await _missionRepository.UpdateAsync(mission);
//             await _missionRepository.AddHistoryAsync(history);

//             return true;
//         }
//     }
// }

//EDIT: DIEU 18/05/2026 - Cập nhật lại logic cho phép hủy nhiệm vụ từ bất kỳ trạng thái nào, nhưng vẫn giữ nguyên quy tắc chuyển trạng thái bình thường cho các trạng thái khác

using MediatR;
using RescueSystem.Application.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Application.Features.Missions.Commands.UpdateMission
{
    public class UpdateMissionHandler : IRequestHandler<UpdateMissionCommand, bool>
    {
        private readonly IMissionRepository _missionRepository;
        public UpdateMissionHandler(IMissionRepository missionRepository)
        {
            _missionRepository = missionRepository;
        }

        public async Task<bool> Handle(UpdateMissionCommand request, CancellationToken cancellationToken)
        {
            var mission = await _missionRepository.GetByIdAsync(request.MissionId);
            if (mission == null)
            {
                throw new Exception("Không tìm thấy nhiệm vụ!");
            }

            // FIXED: Allow abort from any state except already completed/aborted
            if (request.Status == MissionStatus.ABORTED)
            {
                if (mission.Status == MissionStatus.COMPLETED || mission.Status == MissionStatus.ABORTED)
                {
                    return false;
                }
                // Allow abort from any other state
            }
            else if (mission.Status == MissionStatus.COMPLETED || mission.Status == MissionStatus.ABORTED)
            {
                // Cannot update completed/aborted missions
                return false;
            }
            else if (request.Status == MissionStatus.COMPLETED)
            {
                // COMPLETED should only come from FinishMission endpoint
                return false;
            }
            else
            {
                // Validate normal state transitions
                bool isValidTransition = false;

                if (mission.Status == MissionStatus.ASSIGNED && request.Status == MissionStatus.EN_ROUTE)
                    isValidTransition = true;
                else if (mission.Status == MissionStatus.EN_ROUTE && request.Status == MissionStatus.ON_SITE)
                    isValidTransition = true;
                else if (mission.Status == MissionStatus.ON_SITE && request.Status == MissionStatus.IN_PROGRESS)
                    isValidTransition = true;
                else if (mission.Status == MissionStatus.IN_PROGRESS && request.Status == MissionStatus.ON_SITE)
                    isValidTransition = true; // Allow going back

                if (!isValidTransition)
                {
                    return false;
                }
            }

            var previousStatus = mission.Status;
            mission.Status = request.Status;
            mission.UpdatedAt = DateTime.UtcNow.AddHours(7);

            var history = new MissionHistory
            {
                MissionId = mission.Id,
                FromStatus = previousStatus,
                ToStatus = mission.Status,
                ChangedById = request.ChangedById,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow.AddHours(7)
            };

            await _missionRepository.UpdateAsync(mission);
            await _missionRepository.AddHistoryAsync(history);

            return true;
        }
    }
}