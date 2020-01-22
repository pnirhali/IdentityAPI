using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAPI.DTO
{
    public class RegistrationDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
