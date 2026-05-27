using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Contact;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Contact.Queries.GetContactById
{
    public class GetContactByIdHandler : IRequestHandler<GetContactDetailQuery, ContactDTO>
    {
        private readonly IContactRepository _contactRepository;

        public GetContactByIdHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<ContactDTO> Handle(GetContactDetailQuery request, CancellationToken cancellationToken)
        {
            var contact = await _contactRepository.GetByIdAsync(request.Id);

            if (contact == null)
                throw new Exception("Contact không tồn tại");

            if (contact.UserId != request.UserId)
                throw new UnauthorizedAccessException("Không có quyền xem");

            return new ContactDTO
            {
                Id = contact.Id,
                Name = contact.Name,
                Relationship = contact.Relationship,
                PhoneNumber = contact.PhoneNumber,
                Email = contact.Email
            };
        }
    }
}
