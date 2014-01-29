using SmartHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class DevicesListViewModel
    {
        public IEnumerable<DeviceViewModel> Devices { get; set; }

        public int RoomId { get; set; }

    }
}