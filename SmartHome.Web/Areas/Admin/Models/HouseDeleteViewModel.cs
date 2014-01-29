using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class HouseDeleteViewModel
    {
        public string Name { get; set; }

        public string Address { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? DateCreate { get; set; }
    }
}