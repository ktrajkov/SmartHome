using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Models
{
    public class ThermostatEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public bool State { get; set; }

        [Required]
        public bool Behavior { get; set; }

        [Required]
        [Range(-20, 60)]       
        public double TargetTemp { get; set; }
    }
}