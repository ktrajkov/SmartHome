using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Models
{
    public class SensorViewModel
    {
        [Required]
        public int Id { get; set; }
          [DisplayFormat(DataFormatString = "{0:F1}")]
        public double? CurrentTemp { get; set; }

        [Required]
        [Display(Name = "Temp Alarm")]
        public bool AlarmCheck { get; set; }
        public double? MinTempAlert { get; set; }
        public double? MaxTempAlert { get; set; }
    }
}