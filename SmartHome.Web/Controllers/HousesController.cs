using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SmartHome.Web.Models;
using SmartHome.Models;
using System.Web.Security;
using System.Data.Entity;
using SmartHome.Data;
using SmartHome.RemoteControl.Abstract;

namespace SmartHome.Web.Controllers
{
    [Authorize]
    public class HousesController : BaseController
    {
        public HousesController(IUowData data, IRemoteControl remoteControl) : base(data, remoteControl) { }

        [HttpGet]
        public ActionResult List()
        {
            string userId = User.Identity.GetUserId();
            var houses = Data.Houses.All()
                .Where(u => u.UserId == userId)
                .Select(h => new NavigationHouseViewModel
                    {
                        ImageUrl = h.ImageUrl,
                        Id = h.Id,
                        Name=h.Name
                    });
            if (houses.Count() > 1)
            {
                return View(houses);
            }
            else if (houses.Count() == 1)
            {
                return RedirectToAction("Details", new { houseId = houses.SingleOrDefault().Id });
            }
            return View();
        }
        [HttpGet]
        public ActionResult Details(int houseId)
        {
            string userId = User.Identity.GetUserId();
            var houses = this.Data.Houses.All()
                .Where(u => u.UserId == userId && u.Id == houseId).ToList();
            var houseViewModel = houses.Select(h => new HouseViewModel
            {
                Id = h.Id,
                Name = h.Name,
                ImageUrl = h.ImageUrl,
                HouseSettingsViewModel = new HouseSettingsViewModel
                {
                    Id = h.Id,
                    CheckTempTime = h.TimeToCheckTemp,
                    SendTempTime = h.TimeToSendTemp,
                    Email = h.User.Email
                },
                FloorsViewModel = h.Floоrs
                .Select(f => new FloorViewModel
                {
                    Name = f.Name,
                    RoomViewModel = f.Rooms
                    .Select(r => new RoomViewModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        DevicesViewModel = r.Devices
                        .Where(d => d.Id != (d.Room.Thermostat != null ? d.Room.Thermostat.DeviceId:-1))                            
                            .Select(d => new DeviceViewModel
                            {
                                Id = d.Id,
                                Name = d.Name,
                                State = d.State
                            }),
                        SensorViewModel = r.Sensor == null ? null : new SensorViewModel
                        {
                            Id = r.Sensor.Id,
                            CurrentTemp = r.Sensor.CurrentTemp,
                            AlarmCheck = r.Sensor.AlarmCheck,
                            MaxTempAlert = r.Sensor.MaxTempAlert,
                            MinTempAlert = r.Sensor.MinTempAlert,
                        },
                        ThermostatViewModel = r.Thermostat == null ? null : new ThermostatViewModel
                        {
                            Id = r.Thermostat.Id,
                            Behavior = r.Thermostat.Behavior,
                            State = r.Thermostat.State,
                            TargetTemp = r.Thermostat.TargetTemp
                        }
                    })
                })
            }).SingleOrDefault();
            if (houseViewModel==null)
            {
               return  HttpNotFound();
            }
            return View(houseViewModel);
        }

       
    }
}