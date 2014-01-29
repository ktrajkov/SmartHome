using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Models
{
    public class ApplicationUser : User
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"([a-zA-Z_\.]+@[a-zA-Z_]+?\.[a-zA-Z]{2,6})", ErrorMessage = "The email is invalid.")]
        public string Email { get; set; }
    }

}
