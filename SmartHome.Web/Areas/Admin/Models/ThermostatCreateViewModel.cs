using SmartHome.Web.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class ThermostatCreateViewModel
    {
        [Required]
        public bool State { get; set; }

        [Required]
        public bool Behavior { get; set; }

        [RequiredIfTrue("State")]
        [Display(Name = "Target Temp")]
        [ThermostatTempRange]
        public double? TargetTemp { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        [Display(Name = "Device Name")]
        public int DeviceId { get; set; }

        public SelectList Devices { get; set; }
    }
}