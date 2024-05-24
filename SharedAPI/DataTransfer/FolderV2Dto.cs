using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAPI.DataTransfer
{
    public class FolderV2Dto
    {
        public Guid Id { get; set; }

        public Guid BaseFolderId { get; set; }

        public string? Owner { get; set; }

        public string? Name { get; set; }

        public string? Access { get; set; }

        public List<CollaboratorDto>? Collaborators { get; set; }

        public List<FolderDto>? Folders { get; set; }

        public List<ContentDto>? Contents { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
