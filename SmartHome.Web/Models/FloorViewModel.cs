using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartHome.Web.Models
{
    public class FloorViewModel
    {
        public string  Name { get; set; }
        public IEnumerable<RoomViewModel> RoomViewModel { get; set; }
      
    }
}
