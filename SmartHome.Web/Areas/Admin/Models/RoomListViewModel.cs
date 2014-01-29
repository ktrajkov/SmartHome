using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Areas.Admin.Models
{
    public class RoomListViewModel
    {
        public IEnumerable<RoomViewModel> RoomsViewModel { get; set; }
        public int FloorId { get; set; }
        public int HouseId { get; set; }
    }
}