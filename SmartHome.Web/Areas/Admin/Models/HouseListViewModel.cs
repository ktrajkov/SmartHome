using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class HouseListViewModel
    {
        public int Id { get; set; }
        public string  Name { get; set; }

        public string  Adress { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? DateCreated { get; set; }

        public string  Owner { get; set; }
    }
}