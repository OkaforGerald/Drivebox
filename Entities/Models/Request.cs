using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public enum RequestStatus
    {
        Accepted = 100,
        Denied = 200
    }

    public class Request : EntityBase
    {
        public User? Requester { get; set; }
        public string? RequesterId { get; set; }

        public Folder? Folder { get; set; }
        public Guid FolderId { get; set; }

        public RequestStatus Status { get; set; }
    }
}
