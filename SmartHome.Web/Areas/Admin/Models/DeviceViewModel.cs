using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class DeviceViewModel
    {
        public int Id { get; set; }

        public string  Name { get; set; }

        public bool State { get; set; }

        [Display(Name="Attached Pin")]
        public Int16 AttachedPin { get; set; }
      
    }
}