using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class DeviceCreateViewModel
    {
        [Required]
        public Int16 AttachedPin { get; set; }     

        [Required(ErrorMessage = "Field is required and should not be more than 30 characters")]
        [StringLength(30)]
        [Display(Name = "Device name")]
        public string Name { get; set; }

        [Required]
        public bool State { get; set; }

        public SelectList AllowedPins { get; set; }

        [Required]
        public int RoomId { get; set; }

    }
}