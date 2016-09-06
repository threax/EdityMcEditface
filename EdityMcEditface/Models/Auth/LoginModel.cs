using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Models.Auth
{
    public class LoginModel
    {
        [Required]
        public String UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }
    }
}
