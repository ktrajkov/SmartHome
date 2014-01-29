using Microsoft.AspNet.Identity;
using SmartHome.Data;
using SmartHome.Models;
using SmartHome.RemoteControl.Abstract;
using SmartHome.RemoteControl.Models;
using SmartHome.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.Controllers
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class HouseSettingsController : BaseController
    {
        public HouseSettingsController(IUowData data, IRemoteControl remoteControl) : base(data, remoteControl) { }

        [HttpPost]
        public ActionResult EditSettings(HouseSettingsViewModel houseSettingsViewModel)
        {        
            if (ModelState.IsValid)
            {              
                House house = this.Data.Houses.GetById(houseSettingsViewModel.Id);
                if (house == null)
                {
                    throw new HttpException(500, "No house with this id");
                }
                house.TimeToCheckTemp = houseSettingsViewModel.CheckTempTime;
                house.TimeToSendTemp = houseSettingsViewModel.SendTempTime;
                house.User.Email = houseSettingsViewModel.Email;

                HouseUserEditRCModel houseUserEditRCModel = new HouseUserEditRCModel
                {
                    ReceiverIp = house.ReceiverIp,
                    TimeToCheckTemp = houseSettingsViewModel.CheckTempTime,
                    TimeToSendTemp = houseSettingsViewModel.SendTempTime
                };

                using (TransactionScope transaction = new TransactionScope())
                {
                    this.Data.Houses.Update(house);
                    this.Data.SaveChanges();

                    this.RemoteControl.SendHouseSettings(houseUserEditRCModel);
                    transaction.Complete();
                }
                return new HttpStatusCodeResult(200);
            }
            else
            {
                throw new HttpException(500,"The model is invalid");
            }
        }
    }
}