using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHome.RemoteControl.Abstract;
using SmartHome.RemoteControl.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartHome.RemoteControl.Concrete
{
    public class WebMessageSender : IMessageSender
    {        
        public void SendMessageToReceiver(object message, string receiverIp)
        {
            //string requestData = JsonConvert.SerializeObject(message);
            //byte[] data = Encoding.UTF8.GetBytes(requestData);
            //using (var client = new ExtendedWebClient(Settings.Default.WebClientTimeout))
            //{
            //    byte[] result = client.UploadData("http://"+receiverIp, "POST", data);
            //    string resultString = Encoding.UTF8.GetString(result).Trim();
            //    JObject resultJson = JsonConvert.DeserializeObject<JObject>(resultString);
            //    JToken status;
            //    if (resultJson==null || !resultJson.TryGetValue("Status", out status) || !status.ToString().Equals("ok"))
            //    {
            //        throw new Exception("Message Not Send");
            //    }
            //}
        }

    }
}
