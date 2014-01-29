using SmartHome.Web.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Models
{
    public class DateRangeViewModel
    {
        [Required]
        public int SensorId { get; set; }

        [Required]
        [UIHint("DateTime")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "From")]
        [DataType(DataType.Date)]
        [MinDateRange("ToDate")]
        public DateTime FromDate { get; set; }

        [Required]
        [UIHint("DateTime")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name="To")]
        [DataType(DataType.Date)]
        [MaxDateRange("FromDate")]
        public DateTime ToDate { get; set; }
    }
}