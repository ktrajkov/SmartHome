using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Models
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public IEnumerable<DeviceViewModel> DevicesViewModel { get; set; }
        public SensorViewModel SensorViewModel { get; set; }
        public ThermostatViewModel ThermostatViewModel { get; set; }
    }
}