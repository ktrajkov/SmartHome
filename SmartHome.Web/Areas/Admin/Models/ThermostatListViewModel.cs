using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class ThermostatListViewModel
    {
        public ThermostatViewModel ThermostatViewModel { get; set; }

        public int RoomId { get; set; }
    }
}