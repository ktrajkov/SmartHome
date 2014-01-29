using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.RemoteControl.Models
{
    public class BaseDeviceRCModel
    {
        public Int16 Pin { get; set; }

        public bool State { get; set; }
    }
}
