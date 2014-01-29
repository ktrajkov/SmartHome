using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.RemoteControl.Models
{
    public class SensorRCModel
    {
        public string ReceiverIp { get; set; }
        public int Id { get; set; }

        public Int16 AttachedPin { get; set; }

        public bool AlarmCheck { get; set; }

        public double? MinTempAlert { get; set; }

        public double? MaxTempAlert { get; set; }
    }
}
