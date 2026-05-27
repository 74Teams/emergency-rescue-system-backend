using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Http;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.Common.Interfaces.Services;
using RescueSystem.Application.DTOs.Media;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Application.Features.Request.Commands.CreateRequest
{
    public class CreateRequestHandler : IRequestHandler<CreateRequestCommand, RescueRequest>
    {
        IRequestRespository _requestRepository;
        ICloudinaryService _cloudinaryService;
        ILocationRepository _locationRepository;

        private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png", "image/webp", "image/gif" };

        private static readonly string[] AllowedVideoTypes = { "video/mp4", "video/avi", "video/mov", "video/webm" };
        public CreateRequestHandler(IRequestRespository requestRepository, ICloudinaryService cloudinaryService, ILocationRepository locationRepository)
        {
            _requestRepository = requestRepository;
            _cloudinaryService = cloudinaryService;
            _locationRepository = locationRepository;
        }
        public async Task<RescueRequest> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
        {
            var RequestId = Guid.NewGuid();
            var medias = new List<MediaResult>();

            var files = request.Files == null ? new List<IFormFile>() : request.Files;

            var location = await _locationRepository.GetByIdAsync(request.LocationId);
            if (location == null)
                throw new InternalServerErrorException("Vị trí cứu hộ không hợp lệ.");

            foreach (var file in files)
            {
                var isImage = AllowedImageTypes.Contains(file.ContentType);
                var isVideo = AllowedVideoTypes.Contains(file.ContentType);

                if (!isImage && !isVideo)
                    throw new InternalServerErrorException($"File '{file.FileName}' không được hỗ trợ. Kiểu file: {file.ContentType}");

                var maxSize = isImage ? 10 * 1024 * 1024 : 100 * 1024 * 1024;
                if (file.Length > maxSize)
                    throw new InternalServerErrorException($"File '{file.FileName}' vượt quá dung lượng cho phép.");
            }

            var uploadTasks = files.Select(file =>
            {
                var isImage = AllowedImageTypes.Contains(file.ContentType);

                return isImage
                    ? _cloudinaryService.UploadImageAsync(file, cancellationToken)
                    : _cloudinaryService.UploadVideoAsync(file, cancellationToken);
            });

            medias = (await Task.WhenAll(uploadTasks)).ToList();

            var RescueReq = new RescueRequest
            {
                Id = RequestId,
                UserId = request.UserId,
                EmergencyType = request.EmergencyType,
                Priority = request.Priority,
                Status = request.Status,
                LocationId = request.LocationId,
                Description = request.Description,
                Medias = medias.Select(m => new RequestMedia
                {
                    PublicId = m.PublicId,
                    RequestId = RequestId,
                    ResourceType = m.ResourceType == "image" ? MediaType.Image : MediaType.Videos,
                    SecureUrl = m.SecureUrl
                }).ToList()
            };

            await _requestRepository.CreateAsync(RescueReq);

            return RescueReq;
        }


    }
}
