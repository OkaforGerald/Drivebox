using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace SharedAPI.DataTransfer
{
    public class RequestDto
    {
        public Guid Id { get; set; }

        public string? Requester { get; set; }

        public Guid FolderId { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
