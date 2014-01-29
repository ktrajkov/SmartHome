using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Models
{
    public class HouseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public HouseSettingsViewModel HouseSettingsViewModel { get; set; }
        public IEnumerable<FloorViewModel> FloorsViewModel { get; set; }

    }
}