using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public enum FileType
    {
        Docs = 100,
        Images = 200,
        Videos = 300
    }

    public class Content : EntityBase
    {
        public Folder? Folder { get; set; }
        public Guid FolderId { get; set; }

        public string? Name { get; set; }

        public long Size { get; set; }

        public string? FileExt { get; set; }

        public FileType FileType { get; set; }

        public string? URL { get; set; }
    }
}
