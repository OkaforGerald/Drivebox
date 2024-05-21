using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAPI.DataTransfer
{
    public class AuthenticateUserDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set;}

        [Required]
        public string? Password { get; set;}
    }
}
