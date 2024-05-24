using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace SharedAPI.DataTransfer
{
    public class CreateFolderDto
    {
        public Guid BaseFolderId { get; set; } = Guid.Empty;

        [Required]
        public string? Name { get; set; }

        [Required]
        public Access Access { get; set; }
    }
}
