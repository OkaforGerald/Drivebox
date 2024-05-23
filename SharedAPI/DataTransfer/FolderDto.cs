using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace SharedAPI.DataTransfer
{
    public class FolderDto
    {
        public Guid Id { get; set; }

        public Guid BaseFolderId { get; set; }

        public string? Owner { get; set; }

        public string? Name { get; set; }

        public string? Access { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
