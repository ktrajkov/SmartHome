using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.RemoteControl.Abstract
{
    public interface IMessageSender
    {
        void SendMessageToReceiver(object message, string ReceiverIp);
    }
}
