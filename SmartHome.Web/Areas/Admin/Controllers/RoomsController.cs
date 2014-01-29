using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SmartHome.Models;
using SmartHome.Data;
using SmartHome.Web.Areas.Admin.Models;
using SmartHome.Web.Areas.Admin.HelpersClass;
using SmartHome.RemoteControl.Abstract;

namespace SmartHome.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoomsController : BaseController
    {
        public RoomsController(IUowData data, IRemoteControl remoteControl) : base(data, remoteControl) { }
        public ActionResult List(int? floorId)
        {
            if (floorId == null)
            {
                return HttpNotFound();
            }
            var roomListViewModel = this.Data.Floors.All()
               .Where(r => r.Id == floorId)
               .Select(s => new RoomListViewModel
               {
                   HouseId = s.HouseId,
                   FloorId = floorId.Value,
                   RoomsViewModel = s.Rooms.Select(r => new RoomViewModel
                   {
                       Id = r.Id,
                       Name = r.Name
                   })
               }).SingleOrDefault();
            return View(roomListViewModel);
        }

        public ActionResult Create(int? floorId)
        {
            if (floorId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Floor floor=this.Data.Floors.GetById(floorId.Value);
            if (floor == null)
            {
                return HttpNotFound();
            }
            RoomCreateViewModel roomCreateViewModel = new RoomCreateViewModel
            {
                FloorId = floorId.Value,
            };
            return View(roomCreateViewModel);
        }

        [HttpPost]
        public ActionResult Create(RoomCreateViewModel roomCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                Room room = new Room
                {
                    Name = roomCreateViewModel.Name,
                    FloorId = roomCreateViewModel.FloorId
                };
                this.Data.Rooms.Add(room);
                this.Data.SaveChanges();
                return RedirectToAction("List", "Rooms", new { FloorId = roomCreateViewModel.FloorId });
            }
            return View(roomCreateViewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var roomEditViewModel = this.Data.Rooms.All()
                .Where(r => r.Id == id.Value)
                .Select(s => new RoomEditAndDeleteViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    FloorId = s.FloorId
                }).SingleOrDefault();
            if (roomEditViewModel == null)
            {
                return HttpNotFound();
            }
            return View(roomEditViewModel);
        }

        [HttpPost]
        public ActionResult Edit(RoomEditAndDeleteViewModel roomViewModel)
        {
            if (ModelState.IsValid)
            {
                Room room = this.Data.Rooms.GetById(roomViewModel.Id);
                if (room == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                room.Name = roomViewModel.Name;
                this.Data.Rooms.Update(room);
                this.Data.SaveChanges();
                return RedirectToAction("List", "Rooms", new { FloorId = roomViewModel.FloorId });
            }
            return View(roomViewModel);
        }

        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var roomDeleteViewModel = this.Data.Rooms.All()
                .Where(r => r.Id == id)
                .Select(s => new RoomEditAndDeleteViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    FloorId = s.FloorId,
                }).SingleOrDefault();
            if (roomDeleteViewModel == null)
            {
                return HttpNotFound();
            }
            return View(roomDeleteViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Room room = this.Data.Rooms.GetById(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            this.Data.Rooms.Delete(room);
            this.Data.SaveChanges();
            return RedirectToAction("List", "Rooms", new { FloorId = room.FloorId });
        }

        public ActionResult RoomDetails(int roomId)
        {
            var roomDetails = this.Data.Rooms.All()
                .Where(r => r.Id == roomId).ToList()
                .Select(s => new RoomDetailsViewModel
                {
                    DevicesListViewModel = new DevicesListViewModel
                    {
                        Devices = s.Thermostat==null? 
                        s.Devices
                        .Select(d => new DeviceViewModel
                        {
                            Id = d.Id,
                            Name = d.Name,
                            AttachedPin = d.AttachedPin,
                            State = d.State
                        }):
                        s.Devices.Where(d=>d.Id!=d.Room.Thermostat.DeviceId)
                        .Select(d => new DeviceViewModel
                        {
                            Id=d.Id,
                            Name=d.Name,
                            AttachedPin=d.AttachedPin,
                            State=d.State
                        }),
                        RoomId=roomId                       
                    },
                    SensorListViewModel = s.Sensor == null ?
                    new SensorListViewModel{RoomId=roomId}:new SensorListViewModel
                    {
                        SensorViewModel = new SensorViewModel
                        {
                            Id=s.Sensor.Id,
                            AlarmCheck=s.Sensor.AlarmCheck,
                            ArduinoArraySensorsId=s.Sensor.ArduinoArraySensorsId,
                            AttachedPin=s.Sensor.AttachedPin,
                            CurrentTemp=s.Sensor.CurrentTemp,
                            MaxTempAlert=s.Sensor.MaxTempAlert,
                            MinTempAlert=s.Sensor.MinTempAlert
                        },
                        RoomId=roomId
                    },                   
                    ThermostatListViewModel = s.Thermostat == null ? 
                        new ThermostatListViewModel {RoomId=roomId } : new ThermostatListViewModel
                        {
                            RoomId = roomId,
                            ThermostatViewModel = new ThermostatViewModel
                            {
                                Id = s.Thermostat.Id,
                                State = s.Thermostat.State,
                                TargetTemp = s.Thermostat.TargetTemp,
                                Behavior = s.Thermostat.Behavior,
                                ArduinoArrayThermostatId = s.Thermostat.ArduinoArrayTermostatId,
                                DeviceName = s.Thermostat.Device.Name
                            }

                        },
                    FloorId = s.FloorId,
                }).SingleOrDefault();
            if (roomDetails == null)
            {
                return HttpNotFound();
            }
            return View(roomDetails);
        }

    }
}
