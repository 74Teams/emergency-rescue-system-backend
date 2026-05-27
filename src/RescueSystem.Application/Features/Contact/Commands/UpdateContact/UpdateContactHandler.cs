using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Contact;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Contact.Commands.UpdateContact
{
    public class UpdateContactHandler : IRequestHandler<UpdateContactCommand, ContactDTO>
    {
        private readonly IContactRepository _contactRepository;
        public UpdateContactHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<ContactDTO> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _contactRepository.GetByIdAsync(request.Id);

            if (contact == null)
                throw new Exception("Contact không tồn tại");

            if (contact.UserId != request.UserId)
                throw new UnauthorizedAccessException("Không có quyền sửa contact này");

            // update
            contact.Name = request.Name;
            contact.Relationship = request.Relationship;
            contact.PhoneNumber = request.PhoneNumber;
            contact.Email = request.Email;
            contact.UpdatedAt = DateTime.UtcNow;

            await _contactRepository.UpdateAsync(contact);

            return new ContactDTO
            {
                Id = request.Id,
                Name = request.Name,
                Relationship = request.Relationship,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email
            };
        }
    }
}
