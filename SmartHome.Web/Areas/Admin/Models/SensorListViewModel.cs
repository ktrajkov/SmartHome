using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class SensorListViewModel
    {
        public SensorViewModel SensorViewModel { get; set; }
        public int RoomId { get; set; }
    }
}