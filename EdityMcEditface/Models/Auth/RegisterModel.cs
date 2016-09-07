using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Models.Auth
{
    public class RegisterModel
    {
        [Required]
        public String UserName { get; set; }

        [Required]
        public String DisplayName { get; set; }

        [Required]
        [EmailAddress]
        public String Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public String PasswordVerify { get; set; }
    }
}
