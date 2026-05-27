using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.DTOs.Contact;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Contact.Queries.GetAllContact
{
    public class GetAllContactHandler : IRequestHandler<GetAllContactQuery, List<ContactDTO>>
    {
        private readonly IContactRepository _contactRepository;

        public GetAllContactHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<List<ContactDTO>> Handle(GetAllContactQuery request, CancellationToken cancellationToken)
        {
            var contacts = await _contactRepository.GetByUserIdAsync(request.UserId);

            return contacts.Select(c => new ContactDTO
            {
                Id = c.Id,
                Name = c.Name,
                Relationship = c.Relationship,
                PhoneNumber = c.PhoneNumber,
                Email = c.Email
            }).ToList();
        }
    }
}
