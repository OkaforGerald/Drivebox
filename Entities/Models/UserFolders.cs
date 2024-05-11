using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public enum Permissions
    {
        Read = 100,
        ReadnWrite = 200
    }

    public class UserFolders
    {
        public Guid Id { get; set; }

        public User? User { get; set; }
        public string? UserId { get; set; }

        public Folder? Folder { get; set; }
        public Guid FolderId { get; set; }

        public Permissions Permissions { get; set;}
    }
}
