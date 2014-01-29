using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.RemoteControl.Models
{
    public class HouseRCModel
    {
        public string ServerIp { get; set; }
        public int ServerPort { get; set; }
        public string ReceiverIp { get; set; }
        public int TimeToSendTemp { get; set; }
        public int TimeToCheckTemp { get; set; }
    }
}
