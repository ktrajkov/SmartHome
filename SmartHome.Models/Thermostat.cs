using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Models
{
    public class Thermostat
    {

        public int Id { get; set; }

        public bool State { get; set; }

        public bool Behavior { get; set; }

        public double? TargetTemp { get; set; }

        public int ArduinoArrayTermostatId { get; set; }

        public int DeviceId { get; set; }

        public int RoomId { get; set; }

        public int SensorId { get; set; }
        public virtual Device Device { get; set; }

        public virtual Sensor Sensor { get; set; }
        public virtual Room Room { get; set; }













    }
}
