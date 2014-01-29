using SmartHome.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class SensorViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Attached Pin")]
        public Int16 AttachedPin { get; set; }

        [Display(Name = "Current Temp")]      
        public double? CurrentTemp { get; set; }

        [Display(Name = "Arduino Array Sensors Id")]
        public int ArduinoArraySensorsId { get; set; }

        [Display(Name = "Alarm Check")]
        public bool AlarmCheck { get; set; }

        [Display(Name = "Min. Temp. Alert")]
        public double? MinTempAlert { get; set; }

        [Display(Name = "Max. Temp. Alert")]
        public double? MaxTempAlert { get; set; }
       
    }
}