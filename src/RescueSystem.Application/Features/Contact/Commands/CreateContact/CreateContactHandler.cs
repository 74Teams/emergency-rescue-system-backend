using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Contact;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Features.Contact.Commands.CreateContact
{
    public class CreateContactHandler : IRequestHandler<CreateContactCommand, ContactDTO>
    {
        private readonly IContactRepository _contactRepository;

        public CreateContactHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<ContactDTO> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
                throw new Exception("UserId không hợp lệ");

            var contactEntity = new RescueSystem.Domain.Entities.Contact
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Relationship = request.Relationship,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _contactRepository.CreateAsync(contactEntity);

            return new ContactDTO
            {
                Id = contactEntity.Id,
                Name = contactEntity.Name,
                Relationship = contactEntity.Relationship,
                PhoneNumber = contactEntity.PhoneNumber,
                Email = contactEntity.Email
            };
        }
    }
}