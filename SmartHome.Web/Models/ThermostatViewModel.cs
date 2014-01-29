using SmartHome.Web.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Models
{
    public class ThermostatViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public bool State { get; set; }

        [Required]
        [Display(Name = "Mode")]
        public bool Behavior { get; set; }

        [RequiredIfTrue("State")]
        [Range(-20, 60, ErrorMessage = ("-20< Temp >60"))]
        [Display(Name = "Target Temp")]
        public double? TargetTemp { get; set; }

    }
}