using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Entities.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiry { get; set; }

        public ICollection<UserFolders>? UserFolders { get; set; }
    }
}
