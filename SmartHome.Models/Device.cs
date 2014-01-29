using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Models
{
    public class Device
    {
        public int Id { get; set; }


        public string Name { get; set; }

        public bool State { get; set; }

        public Int16 AttachedPin { get; set; }


        public int RoomId { get; set; }


        public virtual Room Room { get; set; }
    }
}