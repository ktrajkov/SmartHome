using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.RemoteControl.Concrete
{
    public class ExtendedWebClient : WebClient
    {
        public int Timeout { get; set; }      
        public ExtendedWebClient(int timeout)
        {
            this.Timeout = timeout;
        }
        protected override WebRequest GetWebRequest(Uri address)
        {
            var objWebRequest = base.GetWebRequest(address);
            objWebRequest.Timeout = this.Timeout;
            return objWebRequest;
        }
    }
}
