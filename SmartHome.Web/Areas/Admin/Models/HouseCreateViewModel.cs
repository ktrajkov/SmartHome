using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class HouseCreateViewModel
    {
        [Required(ErrorMessage = "The field is required and should not be more than 30 characters")]
        [StringLength(32)]
        [Display(Name = "House name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The field is required and should not be more than 30 characters")]
        [StringLength(30)]
        public string Address { get; set; }

        [Display(Name = "Date create")]
        [UIHint("DateTime")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public Nullable<System.DateTime> DateCreate { get; set; }

        [Required(ErrorMessage = "The field is required and should not be more than 50 characters")]
        [StringLength(50)]
        public string SecretKey { get; set; }

        [Required]
        [StringLength(15)]
        [RegularExpression(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b",
            ErrorMessage = @"Ip is not valid. ""(192.168.1.1)""")]
        public string ServerIp { get; set; }

        [Required]
        [Display(Name = "Server Port")]
        public int ServerPort { get; set; }

        [Required]
        [StringLength(15)]
        [RegularExpression(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b",
            ErrorMessage = @"Ip is not valid. ""(192.168.1.1)""")]
        public string ReceiverIp { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int TimeToCheckTemp { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int TimeToSendTemp { get; set; }

        [Required]
        [Range(1, Int16.MaxValue)]
        public Int16 MaxNumberPins { get; set; }

        [Required]
        [Range(1, byte.MaxValue, ErrorMessage = "MaxArraySizeTermostat must be between 1 and 255")]
        public byte MaxArraySizeTermostats { get; set; }

        [Required]
        [Range(1, byte.MaxValue, ErrorMessage = "MaxArraySizeSensor must be between 1 and 255")]
        public byte MaxArraySizeSensors { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserId { get; set; }

        public SelectList Users { get; set; }

    }
}