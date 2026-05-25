using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RescueSystem.Application.Features.ChecklistItem.Commands.UpdateChecklistItem
{
    public class UpdateChecklistItemCommand : IRequest<Unit>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsCheck { get; set; }
    }
}
