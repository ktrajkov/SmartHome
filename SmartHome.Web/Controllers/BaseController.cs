using SmartHome.Models;
using SmartHome.RemoteControl.Concrete;
using SmartHome.RemoteControl.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartHome.Data;

namespace SmartHome.Web.Controllers
{
    public class BaseController:Controller
    {
        protected IUowData Data;
        protected IRemoteControl RemoteControl;       
        public BaseController(IUowData data, IRemoteControl remoteControl)
        {
            this.Data = data;
            this.RemoteControl = remoteControl;
        }

       
    }
}