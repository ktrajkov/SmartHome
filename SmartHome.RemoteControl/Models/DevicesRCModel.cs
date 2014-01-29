using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.RemoteControl.Models
{
    public class DevicesRCModel
    {
        public string ReceiverIp { get; set; }
        public IList<BaseDeviceRCModel> Devices { get; set; }
    }
}
