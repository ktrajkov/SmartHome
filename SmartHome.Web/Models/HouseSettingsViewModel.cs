using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Models
{
    public class HouseSettingsViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Send temp (Min)")]
        public int SendTempTime { get; set; }

        [Required]
        [Display(Name = "Check temp (Min)")]
        public int CheckTempTime { get; set; }

        [Required]
        [Display(Name = "E-mail for Alarm")]
        [RegularExpression(@"([a-zA-Z_\.]+@[a-zA-Z_\.]+?\.[a-zA-Z]{2,6})", ErrorMessage = "The email is invalid.")]
        public string Email { get; set; }
    }
}