using SmartHome.Web.Properties;
using SmartHome.Web.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Web;

namespace SmartHome.Web.Models
{
    public class SensorEditViewModel
    {
        
        [Required]
        public int Id { get; set; }

        [Required]     
        public bool AlarmCheck { get; set; }

        [RequiredIfTrue("AlarmCheck")]
        [MinTempRangeAlert("MaxTempAlert")]
        public double? MinTempAlert { get; set; }

        [RequiredIfTrue("AlarmCheck")]       
        [MaxTempRangeAlert("MinTempAlert")]
        public double? MaxTempAlert { get; set; }
    }
}
