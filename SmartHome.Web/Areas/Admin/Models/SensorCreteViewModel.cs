using SmartHome.Web.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class SensorCreteViewModel
    {
        [Required]
        public Int16 AttachedPin { get; set; }

        [Required]
        public bool AlarmCheck { get; set; }

        [RequiredIfTrue("AlarmCheck")]
        [MinTempRangeAlert("MaxTempAlert")]
        [Display(Name = "Min. Temp. Alert")]
        public double? MinTempAlert { get; set; }

        [RequiredIfTrue("AlarmCheck")]
        [MaxTempRangeAlert("MinTempAlert")]
        [Display(Name = "Max. Temp. Alert")]
        public double? MaxTempAlert { get; set; }

        public SelectList AllowedPins { get; set; }

        [Required]
        public int RoomId { get; set; }

    }
}