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

        public string? Name { get; set; }

        public string? FileExt { get; set; }

        public string? FileType { get; set; }

        public string? Size { get; set; }

        public string? URL { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
