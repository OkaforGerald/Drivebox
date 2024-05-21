using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace SharedAPI.DataTransfer
{
    public class CreateFolderDto
    {
        public string? Name { get; set; }

        public Access Access { get; set; }
    }
}
