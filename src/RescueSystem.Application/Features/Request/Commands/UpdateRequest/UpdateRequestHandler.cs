using MediatR;
using Microsoft.AspNetCore.Http;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.Common.Interfaces.Services;
using RescueSystem.Application.DTOs.Media;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Application.Features.Request.Commands.UpdateRequest
{
    public class UpdateRequestHandler : IRequestHandler<UpdateRequestCommand, RescueRequest>
    {
        private readonly IRequestRespository _requestRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILocationRepository _locationRepository; //EDIT: 30/5 by Dieu

        private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png", "image/webp", "image/gif" };
        private static readonly string[] AllowedVideoTypes = { "video/mp4", "video/avi", "video/mov", "video/webm" };

        public UpdateRequestHandler(IRequestRespository requestRepository, ICloudinaryService cloudinaryService, ILocationRepository locationRepository)
        {
            _requestRepository = requestRepository;
            _cloudinaryService = cloudinaryService;
            _locationRepository = locationRepository;
        }

        public async Task<RescueRequest> Handle(UpdateRequestCommand request, CancellationToken cancellationToken)
        {
            var existingRequest = await _requestRepository.GetByIdAsync(request.RequestId);
            if (existingRequest == null)
            {
                throw new NotFoundException("Không tìm thấy yêu cầu cứu hộ");
            }

            //EDIT: 30/5 by Dieu - Kiểm tra LocationId
            var location = await _locationRepository.GetByIdAsync(request.LocationId);
            if (location == null)
            {
                throw new BadRequestException("Vị trí không hợp lệ");
            }

            var files = request.Files == null ? new List<IFormFile>() : request.Files;
            var medias = new List<MediaResult>();

            foreach (var file in files)
            {
                var isImage = AllowedImageTypes.Contains(file.ContentType);
                var isVideo = AllowedVideoTypes.Contains(file.ContentType);

                if (!isImage && !isVideo)
                {
                    throw new BadRequestException($"File '{file.FileName}' không được hỗ trợ: {file.ContentType}");
                }

                var maxSize = isImage ? 10 * 1024 * 1024 : 100 * 1024 * 1024;
                if (file.Length > maxSize)
                {
                    throw new BadRequestException($"File '{file.FileName}' vượt quá giới hạn kích thước");
                }
            }

            if (files.Count > 0)
            {
                //EDIT: 30/5 by Dieu - Xóa ảnh cũ trên Cloudinary
                if (existingRequest.Medias != null && existingRequest.Medias.Any())
                {
                    foreach (var oldMedia in existingRequest.Medias)
                    {
                        var resourceType = oldMedia.ResourceType == Domain.Enums.MediaType.Image 
                            ? CloudinaryDotNet.Actions.ResourceType.Image 
                            : CloudinaryDotNet.Actions.ResourceType.Video;
                        await _cloudinaryService.DeleteAsync(oldMedia.PublicId, resourceType);
                    }
                    existingRequest.Medias.Clear();
                }

                var uploadTasks = files.Select(file =>
                {
                    var isImage = AllowedImageTypes.Contains(file.ContentType);
                    return isImage
                        ? _cloudinaryService.UploadImageAsync(file, cancellationToken)
                        : _cloudinaryService.UploadVideoAsync(file, cancellationToken);
                });

                medias = (await Task.WhenAll(uploadTasks)).ToList();
            }

            if (request.UserId.HasValue && request.UserId.Value != Guid.Empty)
            {
                existingRequest.UserId = request.UserId.Value;
            }
            existingRequest.EmergencyType = request.EmergencyType;
            existingRequest.Priority = request.Priority;
            // existingRequest.Status = request.Status; //EDIT: 30/5 by Dieu - Bỏ quyền tự ý cập nhật Status
            existingRequest.LocationId = request.LocationId;
            existingRequest.Description = request.Description;
            existingRequest.UpdatedAt = DateTime.UtcNow;

            if (medias.Count > 0)
            {
                existingRequest.Medias = medias.Select(m => new RequestMedia
                {
                    PublicId = m.PublicId,
                    RequestId = existingRequest.Id,
                    ResourceType = m.ResourceType == "image" ? MediaType.Image : MediaType.Videos,
                    SecureUrl = m.SecureUrl
                }).ToList();
            }

            await _requestRepository.UpdateAsync(existingRequest);
            return existingRequest;
        }
    }
}
