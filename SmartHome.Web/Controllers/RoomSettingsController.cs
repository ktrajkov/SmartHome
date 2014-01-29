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
using System.Web.Script.Serialization;

namespace SmartHome.Web.Controllers
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class RoomSettingsController : BaseController
    {
        public RoomSettingsController(IUowData data, IRemoteControl remoteControl) : base(data, remoteControl) { }

        [HttpPost]
        public ActionResult EditDevices(IEnumerable<DeviceViewModel> devicesViewModel)
        {
            DevicesRCModel devicesRCModel = new DevicesRCModel
            {
                Devices = new List<BaseDeviceRCModel>()
            };          
            using (TransactionScope transaction = new TransactionScope())
            {
                bool isFirst = true;
                foreach (var deviceViewModel in devicesViewModel)
                {
                    Device device = null;
                    if(isFirst)
                    {
                        var deviceModel = this.Data.Devices.All()
                            .Where(d => d.Id == deviceViewModel.Id)
                            .Select(d => new
                            {
                                device = d,
                                ReceiverIp = d.Room.Floor.House.ReceiverIp

                            }).SingleOrDefault();
                        if (deviceModel == null)
                        {
                            throw new HttpException(500, "No device with this Id");
                        }
                        device = deviceModel.device;
                        devicesRCModel.ReceiverIp = deviceModel.ReceiverIp;
                        isFirst = false;
                    }
                    else
                    {
                       device = this.Data.Devices.GetById(deviceViewModel.Id);
                        if (device == null)
                        {
                            throw new HttpException(500, "No device with this Id");
                        }
                    }
                    device.State = deviceViewModel.State;

                    devicesRCModel.Devices.Add(new BaseDeviceRCModel
                    {
                        Pin = device.AttachedPin,
                        State = device.State
                    });
                    this.Data.Devices.Update(device);
                }
                this.Data.SaveChanges();

                if (devicesRCModel.Devices.Count > 0)
                {
                    this.RemoteControl.SendDevicesSettings(devicesRCModel);
                }
                transaction.Complete();
            }
            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult EditSensor(SensorEditViewModel sensorEditViewModel)
        {
            if (ModelState.IsValid)
            {
                var editModel = this.Data.Sensors.All()
                    .Where(s => s.Id == sensorEditViewModel.Id)
                    .Select(s => new
                    {
                        Sensor = s,
                        ReceiverIp = s.Room.Floor.House.ReceiverIp
                    }).SingleOrDefault();
                if (editModel == null)
                {
                    throw new HttpException(500, "No sensor with this Id");
                }
                editModel.Sensor.AlarmCheck = sensorEditViewModel.AlarmCheck;
                editModel.Sensor.MaxTempAlert = sensorEditViewModel.MaxTempAlert;
                editModel.Sensor.MinTempAlert = sensorEditViewModel.MinTempAlert;

                SensorUserEditRCModel sensorUserEditRCModel = new SensorUserEditRCModel
                {
                    Id = editModel.Sensor.ArduinoArraySensorsId,
                    AlarmCheck = editModel.Sensor.AlarmCheck,
                    ReceiverIp = editModel.ReceiverIp,
                    MaxTempAlert = editModel.Sensor.MaxTempAlert,
                    MinTempAlert = editModel.Sensor.MinTempAlert
                };

                using (TransactionScope transaction = new TransactionScope())
                {
                    this.Data.Sensors.Update(editModel.Sensor);
                    this.Data.SaveChanges();

                    this.RemoteControl.SendSensorSettings(sensorUserEditRCModel);
                    transaction.Complete();
                }
                return new HttpStatusCodeResult(200);
            }
            else
            {
                throw new HttpException(500, "The model is invalid");
            }

        }

        [HttpPost]
        public ActionResult EditThermostat(ThermostatViewModel thermostatViewModel)
        {
            if (ModelState.IsValid)
            {
                var editModel = this.Data.Thermostats.All()
                    .Where(t => t.Id == thermostatViewModel.Id)
                    .Select(s => new
                    {
                        Thermostat = s,
                        ReceiverIp = s.Room.Floor.House.ReceiverIp

                    }).SingleOrDefault();
                if (editModel == null)
                {
                    throw new HttpException(500, "No thermostat with this id");
                }
                editModel.Thermostat.State = thermostatViewModel.State;
                editModel.Thermostat.Behavior = thermostatViewModel.Behavior;
                editModel.Thermostat.TargetTemp = thermostatViewModel.TargetTemp;

                ThermostatUserEditRCModel thermostatUserEditRCModel = new ThermostatUserEditRCModel
                {
                    Id = editModel.Thermostat.ArduinoArrayTermostatId,
                    State = editModel.Thermostat.State,
                    TargetTemp = editModel.Thermostat.TargetTemp,
                    Behavior = editModel.Thermostat.Behavior,
                    ReceiverIp = editModel.ReceiverIp
                };

                using (TransactionScope transaction = new TransactionScope())
                {
                    this.Data.Thermostats.Update(editModel.Thermostat);
                    this.Data.SaveChanges();

                    this.RemoteControl.SendThermostatSettings(thermostatUserEditRCModel);
                    transaction.Complete();
                }
                return new HttpStatusCodeResult(200);
            }
            else
            {
                throw new HttpException(500, "The model is invalid");
            }
        }
    }
}