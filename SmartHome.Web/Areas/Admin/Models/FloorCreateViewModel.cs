using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class FloorCreateViewModel
    {

        [Required(ErrorMessage = "Field is required and should not be more than 30 characters")]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        public int HouseId { get; set; }
    }
}