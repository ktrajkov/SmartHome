using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class HouseClearEEPROMVIewModel
    {
        public int Id { get; set; }

        [Display(Name="House Name")]
        public string  Name { get; set; }
    }
}