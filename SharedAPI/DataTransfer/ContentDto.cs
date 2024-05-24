using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAPI.DataTransfer
{
    public class ContentDto
    {
        public Guid Id { get; set; }

        public Guid FolderId { get; set; }

        public string? Name { get; set; }

        public string? FileExt { get; set; }

        public string? FileType { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
