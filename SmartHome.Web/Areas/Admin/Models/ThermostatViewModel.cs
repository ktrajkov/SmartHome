using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartHome.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class ThermostatViewModel
    {
        public int Id { get; set; }

        public bool State { get; set; }

        public bool Behavior { get; set; }

        [Display(Name = "Target Temp")]
        public double? TargetTemp { get; set; }

        [Display(Name = "Arduino Array Thermostat Id")]
        public int ArduinoArrayThermostatId { get; set; }

        [Display(Name="Device Name")]
        public string DeviceName { get; set; }       
       
    }
}