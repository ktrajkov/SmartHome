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
using SmartHome.RemoteControl.Models;
using System.Transactions;
using SmartHome.RemoteControl.Abstract;

namespace SmartHome.Web.Areas.Admin.Controllers
{
  
    public class ThermostatsController : BaseController
    {
        public ThermostatsController(IUowData data, IRemoteControl remoteControl) : base(data, remoteControl) { }
        public ActionResult Create(int roomId)
        {
            var room = this.Data.Rooms.All()
               .Where(r => r.Id == roomId)
               .Select(r => new
               {

                   ThermostatId = r.ThermostatId,
                   SensorId = r.SensorId,
                   Devices = r.Devices.Select(d => new
                    {
                        Id = d.Id,
                        Name = d.Name
                    }),
                   r.Floor.HouseId
               }).SingleOrDefault();
            if (room == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!HelperClass.CheckArraySizeThermostats(this.Data, room.HouseId))
            {
                return PartialView("_ErrorMessagePartial", "The number of thermostats for this house has reached maximum value");
            }
            if (room.ThermostatId != null)
            {
                return PartialView("_ErrorMessagePartial", "Аlready exists thermostat for this room ");
            }
            if (room.SensorId == null || !(room.Devices.Count() > 0))
            {
                return PartialView("_ErrorMessagePartial", "Тhis room has no sensor or deveices");
            }
            var thermostatCreateViewModel = new ThermostatCreateViewModel
                {
                    RoomId = roomId,
                    Devices = new SelectList(room.Devices, "Id", "Name")
                };

            return View(thermostatCreateViewModel);
        }

        [HttpPost]
        public ActionResult Create(ThermostatCreateViewModel thermostatCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                var createModel = this.Data.Rooms.All()
                   .Where(r => r.Id == thermostatCreateViewModel.RoomId)
                   .Select(s => new
                   {
                       Room = s,
                       ThermostatDevicePin = s.Devices
                            .Where(d => d.Id == thermostatCreateViewModel.DeviceId)
                            .FirstOrDefault().AttachedPin,
                       HouseId = s.Floor.HouseId,
                       ThermostatCounts = s.Floor.Rooms.Where(r => r.Thermostat != null).Count(),
                       SensorIdInArray = s.Sensor.ArduinoArraySensorsId,
                       ReceiverIp = s.Floor.House.ReceiverIp
                   })
                   .SingleOrDefault();
                if (createModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Thermostat thermostat = new Thermostat
                {
                    SensorId = createModel.Room.SensorId.Value,
                    Behavior = thermostatCreateViewModel.Behavior,
                    DeviceId = thermostatCreateViewModel.DeviceId,
                    RoomId = thermostatCreateViewModel.RoomId,
                    State = thermostatCreateViewModel.State,
                    TargetTemp = thermostatCreateViewModel.TargetTemp,
                    ArduinoArrayTermostatId = createModel.ThermostatCounts + 1
                };
                ThermostatRCModel thermostatRCModel = new ThermostatRCModel
                {
                    ReceiverIp = createModel.ReceiverIp,
                    Id = thermostat.ArduinoArrayTermostatId,
                    State = thermostat.State,
                    Behavior = thermostat.Behavior,
                    TargetTemp = thermostat.TargetTemp,
                    SensorId = createModel.SensorIdInArray,
                    TermostatDevicePin = createModel.ThermostatDevicePin,

                };
                createModel.Room.Thermostat = thermostat;

                using (TransactionScope transaction = new TransactionScope())
                {
                    this.Data.Rooms.Update(createModel.Room);
                    this.Data.SaveChanges();

                    this.RemoteControl.SendThermostatSettings(thermostatRCModel);
                    transaction.Complete();
                }

                return RedirectToAction("RoomDetails", "Rooms", new { RoomId = thermostat.RoomId });
            }
            thermostatCreateViewModel.Devices = HelperClass.GetDevicesInRoom(this.Data, thermostatCreateViewModel.RoomId, thermostatCreateViewModel.DeviceId);
            return View(thermostatCreateViewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var thermostat = this.Data.Thermostats.All()
                .Where(t => t.Id == id.Value)
                .Select(t => new
               {
                   State = t.State,
                   Behavior = t.Behavior,
                   TargetTemp = t.TargetTemp,
                   DeviceId = t.DeviceId,
                   Devices = t.Room.Devices.Select(d => new
                   {
                       Id = d.Id,
                       Name = d.Name
                   }),
                   RoomId = t.RoomId
               }).SingleOrDefault();
            if (thermostat == null)
            {
                return HttpNotFound();
            }
            ThermostatEditViewModel thermostatEditViewModel = new ThermostatEditViewModel()
            {
                Id = id.Value,
                State = thermostat.State,
                Behavior = thermostat.Behavior,
                TargetTemp = thermostat.TargetTemp,
                DeviceId = thermostat.DeviceId,
                Devices = new SelectList(thermostat.Devices, "Id", "Name", thermostat.DeviceId),
                RoomId = thermostat.RoomId
            };
            return View(thermostatEditViewModel);
        }

        [HttpPost]
        public ActionResult Edit(ThermostatEditViewModel thermostatEditViewModel)
        {
            if (ModelState.IsValid)
            {
                var editModel = this.Data.Thermostats.All()
                    .Where(t => t.Id == thermostatEditViewModel.Id)
                    .Select(s => new
                    {
                        Thermostat = s,
                        TermostatDevicePin = s.Device.AttachedPin,
                        SensorIdInArray = s.Sensor.ArduinoArraySensorsId,
                        ReceiverIp = s.Room.Floor.House.ReceiverIp
                    })
                    .SingleOrDefault();
                if (editModel == null)
                {
                    return HttpNotFound();
                }
                editModel.Thermostat.Behavior = thermostatEditViewModel.Behavior;
                editModel.Thermostat.DeviceId = thermostatEditViewModel.DeviceId;
                editModel.Thermostat.State = thermostatEditViewModel.State;
                editModel.Thermostat.TargetTemp = thermostatEditViewModel.TargetTemp;

                ThermostatRCModel thermostatRCModel = new ThermostatRCModel
                {
                    ReceiverIp = editModel.ReceiverIp,
                    Id = editModel.Thermostat.ArduinoArrayTermostatId,
                    State = editModel.Thermostat.State,
                    Behavior = editModel.Thermostat.Behavior,
                    TargetTemp = editModel.Thermostat.TargetTemp,
                    SensorId = editModel.SensorIdInArray,
                    TermostatDevicePin = editModel.TermostatDevicePin
                };

                using (TransactionScope transaction = new TransactionScope())
                {
                    this.Data.Thermostats.Update(editModel.Thermostat);
                    this.Data.SaveChanges();

                    thermostatRCModel.TermostatDevicePin = editModel.TermostatDevicePin;
                    this.RemoteControl.SendThermostatSettings(thermostatRCModel);
                    transaction.Complete();
                }

                return RedirectToAction("RoomDetails", "Rooms", new { RoomId = thermostatEditViewModel.RoomId });
            }
            thermostatEditViewModel.Devices = HelperClass.GetDevicesInRoom(this.Data, thermostatEditViewModel.RoomId, thermostatEditViewModel.DeviceId);
            return View(thermostatEditViewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thermostat thermostat = this.Data.Thermostats.GetById(id.Value);
            if (thermostat == null)
            {
                return HttpNotFound();
            }
            return View(thermostat);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var deleteModel = this.Data.Thermostats.All()
                .Where(t => t.Id == id)
                .Select(s => new
                {
                    ThermostatId = s.Id,
                    ThermostatIdInArray = s.ArduinoArrayTermostatId,
                    Room = s.Room,
                    ReceiverIp = s.Room.Floor.House.ReceiverIp,
                }).SingleOrDefault();
            if (deleteModel == null)
            {
                return HttpNotFound();
            }

            ThermostatDeleteRCModel thermostatDeleteRCModel = new ThermostatDeleteRCModel
            {
                ReceiverIp = deleteModel.ReceiverIp,
                Id = deleteModel.ThermostatIdInArray,
            };

            using (TransactionScope transaction = new TransactionScope())
            {
                this.Data.Thermostats.Delete(deleteModel.ThermostatId);
                this.Data.SaveChanges();

                this.RemoteControl.SendThermostatDelete(thermostatDeleteRCModel);
                transaction.Complete();
            }
            return RedirectToAction("RoomDetails", "Rooms", new { RoomId = deleteModel.Room.Id });
        }

    }

}
