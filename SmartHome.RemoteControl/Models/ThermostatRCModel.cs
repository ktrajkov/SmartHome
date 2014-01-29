using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.RemoteControl.Models
{
    public class ThermostatRCModel
    {
        public string ReceiverIp { get; set; }

        public int Id { get; set; }

        public bool State { get; set; }

        public bool Behavior { get; set; }

        public double? TargetTemp { get; set; }

        public int SensorId { get; set; }

        public Int16 TermostatDevicePin { get; set; }
    }
}
