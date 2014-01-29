using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class HouseEditViewModel:HouseCreateViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}