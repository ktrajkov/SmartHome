using SmartHome.Web.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class ThermostatEditViewModel:ThermostatCreateViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}