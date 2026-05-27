using FluentValidation;
using MediatR;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Contact.Commands.DeleteContact
{
    public class DeleteContactHandler : IRequestHandler<DeleteContactCommand, bool>
    {
        private readonly IContactRepository _contactRepository;

        public DeleteContactHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<bool> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _contactRepository.GetByIdAsync(request.Id);

            if (contact == null)
                throw new Exception("Contact không tồn tại");

            if (contact.UserId != request.UserId)
                throw new UnauthorizedAccessException("Không có quyền xóa");

            await _contactRepository.DeleteAsync(contact);
            return true;
        }

    }
}
