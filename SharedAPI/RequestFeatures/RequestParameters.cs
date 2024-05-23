using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAPI.RequestFeatures
{
    public class RequestParameters
    {
        public string? SearchTerm { get; set; }

        public string? OrderBy { get; set; }

        public string? FolderType { get; set; }
    }
}
