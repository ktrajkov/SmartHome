using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class FloorListViewModel
    {
        public IEnumerable<FloorViewModel> Floors { get; set; }
        public int HouseId { get; set; }
    }
}