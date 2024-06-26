﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public enum Access
    {
        Public = 100,
        Private = 200
    }

    public class Folder : EntityBase
    {
        public Guid BaseFolderId { get; set; }

        public User? Owner { get; set; }
        public string? OwnerId { get; set; }

        public string? Name { get; set; }

        public Access Access { get; set; }

        public bool IsOnLocal { get; set; }

        public string? PathOnLocal { get; set; }

        public ICollection<Request>? Requests { get; set; }

        public ICollection<UserFolders>? UserFolders { get; set;}
    }
}
