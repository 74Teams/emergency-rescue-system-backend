using MediatR;
using Microsoft.AspNetCore.Http;
using RescueSystem.Application.Common.Interfaces.Services;
using RescueSystem.Application.DTOs.Avatar;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace RescueSystem.Application.Features.Auth.Commands.UpdateAvatar
{
    public class UpdateAvatarHandler : IRequestHandler<UpdateAvatarCommand, AvatarDTO>
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateAvatarHandler(
            ICloudinaryService cloudinaryService,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _cloudinaryService = cloudinaryService;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AvatarDTO> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new Exception("Unauthorized");

            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            if (request.File == null || request.File.Length == 0)
                throw new Exception("File is empty");

            if (!request.File.ContentType.StartsWith("image/"))
                throw new Exception("Only image is allowed");

            if (request.File.Length > 5 * 1024 * 1024)
                throw new Exception("File must be < 5MB");

            var uploadResult = await _cloudinaryService.UploadImageAsync(request.File, cancellationToken);

            user.Avatar = uploadResult.SecureUrl;
            user.AvatarPublicId = uploadResult.PublicId;

            await _userRepository.UpdateUserAsync(user);

            return new AvatarDTO
            {
                Avatar = uploadResult.SecureUrl,
                PublicId = uploadResult.PublicId
            };
        }
    }
}
