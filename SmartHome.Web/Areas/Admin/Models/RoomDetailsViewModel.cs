using SmartHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class RoomDetailsViewModel
    {
        public DevicesListViewModel DevicesListViewModel { get; set; }

        public ThermostatListViewModel ThermostatListViewModel { get; set; }

        public SensorListViewModel SensorListViewModel { get; set; }

        public int FloorId { get; set; }

    }
}