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
    [Authorize(Roles = "Admin")]
    public class SensorsController : BaseController
    {
        public SensorsController(IUowData data, IRemoteControl remoteControl) : base(data, remoteControl) { }
        public ActionResult Create(int roomId)
        {
            var room = this.Data.Rooms.All()
                .Where(r => r.Id == roomId)
               .Select(r => new
               {
                   SensorId = r.SensorId,
                   HouseId = r.Floor.HouseId
               }).SingleOrDefault();
            if (room == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!HelperClass.CheckArraySizeSensors(this.Data, room.HouseId))
            {
                return PartialView("_ErrorMessagePartial", "The number of sensors for this house has reached maximum value");
            }
            if (room.SensorId != null)
            {
                return PartialView("_ErrorMessagePartial", "Аlready exists sensor for this room ");
            }
            var allowedPin = HelperClass.GetAllowedPins(this.Data, room.HouseId, null);
            if (!(allowedPin.Count() > 0))
            {
                return PartialView("_ErrorMessagePartial", "Тhere are no free pins");
            }
            SensorCreteViewModel sensorCreteViewModel = new SensorCreteViewModel
            {
                AllowedPins = new SelectList(allowedPin),
                RoomId = roomId
            };
            return View(sensorCreteViewModel);
        }

        [HttpPost]
        public ActionResult Create(SensorCreteViewModel sensorCreteViewModel)
        {
            if (ModelState.IsValid)
            {
                var createModel = this.Data.Rooms.All()
                    .Where(r => r.Id == sensorCreteViewModel.RoomId)
                     .Select(s => new
                     {
                         Room = s,
                         SensorCounts = s.Floor.Rooms.Where(r => r.Sensor != null).Count(),
                         HouseId = s.Floor.HouseId,
                         ReceiverIp = s.Floor.House.ReceiverIp
                     })
                    .SingleOrDefault();
                if (createModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Sensor sensor = new Sensor
                {
                    AlarmCheck = sensorCreteViewModel.AlarmCheck,
                    AttachedPin = sensorCreteViewModel.AttachedPin,
                    MaxTempAlert = sensorCreteViewModel.MaxTempAlert,
                    MinTempAlert = sensorCreteViewModel.MinTempAlert,
                    RoomId = sensorCreteViewModel.RoomId,
                    ArduinoArraySensorsId = createModel.SensorCounts + 1
                };

                SensorRCModel sensorRCModel = new SensorRCModel
                {
                    ReceiverIp = createModel.ReceiverIp,
                    Id = createModel.SensorCounts + 1,
                    AttachedPin = sensorCreteViewModel.AttachedPin,
                    AlarmCheck = sensorCreteViewModel.AlarmCheck,
                    MinTempAlert = sensorCreteViewModel.MinTempAlert,
                    MaxTempAlert = sensorCreteViewModel.MinTempAlert,
                };
                createModel.Room.Sensor = sensor;

                using (TransactionScope transaction = new TransactionScope())
                {
                    this.Data.Rooms.Update(createModel.Room);
                    this.Data.SaveChanges();

                    this.RemoteControl.SendSensorSettings(sensorRCModel);
                    transaction.Complete();
                }

                return RedirectToAction("RoomDetails", "Rooms", new { RoomId = sensor.RoomId });
            }
            int houseId = this.Data.Rooms.All()
                .Where(r => r.Id == sensorCreteViewModel.RoomId)
                .Select(s => s.Floor.HouseId).SingleOrDefault();
            var allowedPin = HelperClass.GetAllowedPins(this.Data, houseId, null);
            sensorCreteViewModel.AllowedPins = new SelectList(allowedPin);
            return View(sensorCreteViewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sensor = this.Data.Sensors.All()
                .Where(s => s.Id == id.Value)
                .Select(s => new
               {
                   Id = s.Id,
                   AlarmCheck = s.AlarmCheck,
                   AttachedPin = s.AttachedPin,
                   MaxTempAlert = s.MaxTempAlert,
                   MinTempAlert = s.MinTempAlert,
                   RoomId = s.RoomId,
                   HouseId = s.Room.Floor.HouseId
               }).SingleOrDefault();
            if (sensor == null)
            {
                return HttpNotFound();
            }
            var allowedPin = HelperClass.GetAllowedPins(this.Data, sensor.HouseId, sensor.AttachedPin);
            SensorEditViewModel sensorEditViewModel = new SensorEditViewModel
            {
                AlarmCheck = sensor.AlarmCheck,
                AttachedPin = sensor.AttachedPin,
                Id = sensor.Id,
                MaxTempAlert = sensor.MaxTempAlert,
                MinTempAlert = sensor.MinTempAlert,
                RoomId = sensor.RoomId,
                AllowedPins = new SelectList(allowedPin, sensor.AttachedPin)
            };
            return View(sensorEditViewModel);
        }

        [HttpPost]
        public ActionResult Edit(SensorEditViewModel sensorEditViewModel)
        {
            if (ModelState.IsValid)
            {
                var editModel = this.Data.Sensors.All()
                .Where(s => s.Id == sensorEditViewModel.Id)
                .Select(s => new
                {
                    Sensor = s,
                    ReceiverIp = s.Room.Floor.House.ReceiverIp,
                }).SingleOrDefault();
                if (editModel == null)
                {
                    return HttpNotFound();
                }
                editModel.Sensor.AlarmCheck = sensorEditViewModel.AlarmCheck;
                editModel.Sensor.MaxTempAlert = sensorEditViewModel.MaxTempAlert;
                editModel.Sensor.MinTempAlert = sensorEditViewModel.MinTempAlert;
                editModel.Sensor.AttachedPin = sensorEditViewModel.AttachedPin;

                SensorRCModel sensorRCModel = new SensorRCModel
                {
                    ReceiverIp = editModel.ReceiverIp,
                    Id = editModel.Sensor.ArduinoArraySensorsId,
                    AttachedPin = sensorEditViewModel.AttachedPin,
                    AlarmCheck = sensorEditViewModel.AlarmCheck,
                    MinTempAlert = sensorEditViewModel.MinTempAlert,
                    MaxTempAlert = sensorEditViewModel.MaxTempAlert,
                };

                using (TransactionScope transaction = new TransactionScope())
                {
                    this.Data.Sensors.Update(editModel.Sensor);
                    this.Data.SaveChanges();

                    this.RemoteControl.SendSensorSettings(sensorRCModel);
                    transaction.Complete();
                }

                return RedirectToAction("RoomDetails", "Rooms", new { RoomId = sensorEditViewModel.RoomId });
            }
            int houseId = this.Data.Rooms.All()
               .Where(r => r.Id == sensorEditViewModel.RoomId)
               .Select(s => s.Floor.HouseId).SingleOrDefault();
            var allowedPin = HelperClass.GetAllowedPins(this.Data, houseId, null);
            sensorEditViewModel.AllowedPins = new SelectList(allowedPin);
            return View(sensorEditViewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sensor sensor = this.Data.Sensors.GetById(id.Value);
            if (sensor == null)
            {
                return HttpNotFound();
            }
            return View(sensor);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var deleteModel = this.Data.Sensors.All()
                .Where(t => t.Id == id)
                .Select(s => new
            {
                ReceiverIp = s.Room.Floor.House.ReceiverIp,
                SensorId = s.Id,
                SensorIdInArray = s.ArduinoArraySensorsId,
                Thermostat = s.Room.Thermostat,
                Room = s.Room,
            }).SingleOrDefault();
            if (deleteModel == null)
            {
                return HttpNotFound();
            }
            SensorDeleteRCModel sensorDeleteRCModel = new SensorDeleteRCModel
            {
                Id = deleteModel.SensorIdInArray,
                ReceiverIp = deleteModel.ReceiverIp
            };

            using (TransactionScope transaction = new TransactionScope())
            {
                ThermostatDeleteRCModel thermostatDeleteRCModel = null;
                if (deleteModel.Room.ThermostatId != null)
                {
                    thermostatDeleteRCModel = new ThermostatDeleteRCModel
                    {
                        Id = deleteModel.Thermostat.ArduinoArrayTermostatId,
                        ReceiverIp = deleteModel.ReceiverIp
                    };
                    this.Data.Thermostats.Delete(deleteModel.Room.ThermostatId.Value);
                }
                this.Data.Sensors.Delete(deleteModel.SensorId);
                this.Data.SaveChanges();

                if (thermostatDeleteRCModel != null)
                {
                    this.RemoteControl.SendThermostatDelete(thermostatDeleteRCModel);
                }
                this.RemoteControl.SendSensorDelete(sensorDeleteRCModel);
                transaction.Complete();
            }

            return RedirectToAction("RoomDetails", "Rooms", new { RoomId = deleteModel.Room.Id });
        }

    }
}
