using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Backup : EntityBase
    {
        public User? Owner { get; set; }
        public string? OwnerId { get; set; }

        public Folder? Folder { get; set; }
        public Guid FolderId { get; set; }

        public string? Path { get; set; }

        public string? HashedContents { get; set; }

        public string? HashedFolders { get; set; }
    }
}
