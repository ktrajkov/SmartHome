using SmartHome.RemoteControl.Abstract;
using SmartHome.RemoteControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.RemoteControl.Concrete
{
    public class ArduinoRemoteControl : IRemoteControl
    {
        private readonly IMessageSender Sender;

        public ArduinoRemoteControl(IMessageSender sender)
        {
            this.Sender = sender;
        }

        public void SendHouseSettings(HouseRCModel houseRCModel)
        {
            this.Sender.SendMessageToReceiver(new
            {
                SetConfiguration = new
                {
                    ServerIp = houseRCModel.ServerIp.Replace('.', ','),
                    ServerPort=houseRCModel.ServerPort,
                    TimeToCheckTemp = houseRCModel.TimeToCheckTemp,
                    TimeToSendTemp = houseRCModel.TimeToSendTemp
                }
            }, houseRCModel.ReceiverIp);
        }

        public void SendHouseSettings(HouseUserEditRCModel houseRCModel)
        {
            this.Sender.SendMessageToReceiver(new
            {
                SetConfiguration = new
                {
                    TimeToCheckTemp = houseRCModel.TimeToCheckTemp,
                    TimeToSendTemp = houseRCModel.TimeToSendTemp
                }
            }, houseRCModel.ReceiverIp);
        }

        public void SendDeviceSettings(DeviceRCModel deviceRCModel)
        {
            this.Sender.SendMessageToReceiver(new
            {
                SetDevice = new
                {
                    Pin = deviceRCModel.Pin,
                    State = deviceRCModel.State
                }
            }, deviceRCModel.ReceiverIp);            
        }

        public void SendDevicesSettings(DevicesRCModel devicesRCModel)
        {

            this.Sender.SendMessageToReceiver(new
            {
                SetDevices = devicesRCModel.Devices
            }, devicesRCModel.ReceiverIp);
        }

        public void SendDeviceDelete(DeviceDeleteRCModel deviceDeleteRCModel)
        {
            this.Sender.SendMessageToReceiver(new
            {
                SetDeleteDevice = new
                {
                    Pin = deviceDeleteRCModel.AttachedPin,
                }
            }, deviceDeleteRCModel.ReceiverIp);
        }

        public void SendSensorSettings(SensorRCModel sensorRCModel)
        {
            var modelSensor = new
           {
               SetSensor = new Dictionary<string, object>()
           };
            modelSensor.SetSensor.Add("SensorId", sensorRCModel.Id);
            modelSensor.SetSensor.Add("SensorPin", sensorRCModel.AttachedPin);
            modelSensor.SetSensor.Add("AlarmCheck",sensorRCModel.AlarmCheck);

            if (sensorRCModel.AlarmCheck)
            {
                modelSensor.SetSensor.Add("MinTempAlert", sensorRCModel.MinTempAlert);
                modelSensor.SetSensor.Add("MaxTempAlert",sensorRCModel.MaxTempAlert);
            }
            this.Sender.SendMessageToReceiver(modelSensor, sensorRCModel.ReceiverIp);
        }

        public void SendSensorSettings(SensorUserEditRCModel sensorRCModel)
        {
            var modelSensor = new
            {
                SetSensor = new Dictionary<string, object>()
            };
            modelSensor.SetSensor.Add("SensorId", sensorRCModel.Id);
            modelSensor.SetSensor.Add("AlarmCheck", sensorRCModel.AlarmCheck);

            if (sensorRCModel.AlarmCheck)
            {
                modelSensor.SetSensor.Add("MinTempAlert", sensorRCModel.MinTempAlert);
                modelSensor.SetSensor.Add("MaxTempAlert", sensorRCModel.MaxTempAlert);
            }
            this.Sender.SendMessageToReceiver(modelSensor, sensorRCModel.ReceiverIp);
        }

        public void SendSensorDelete(SensorDeleteRCModel sensorDeleteRCModel)
        {
            this.Sender.SendMessageToReceiver(new
            {
                SetDeleteSensor = new
                {
                    SensorId = sensorDeleteRCModel.Id
                }
            }, sensorDeleteRCModel.ReceiverIp);
        }

        public void SendThermostatSettings(ThermostatRCModel thermostatRCModel)
        {
            var modelThermostat = new
            {
                SetTermostat = new Dictionary<string, object>()
            };
            modelThermostat.SetTermostat.Add("TermostatId", thermostatRCModel.Id);
            modelThermostat.SetTermostat.Add("SensorId", thermostatRCModel.SensorId);
            modelThermostat.SetTermostat.Add("TermostatDevicePin", thermostatRCModel.TermostatDevicePin);
            modelThermostat.SetTermostat.Add("TermostatState", thermostatRCModel.State);

            if (thermostatRCModel.State)
            {
                modelThermostat.SetTermostat.Add("Behavior", thermostatRCModel.Behavior);
                modelThermostat.SetTermostat.Add("TargetTemp", thermostatRCModel.TargetTemp);
            }
            this.Sender.SendMessageToReceiver(modelThermostat, thermostatRCModel.ReceiverIp);
        }

        public void SendThermostatSettings(ThermostatUserEditRCModel thermostatRCModel)
        {
            var modelThermostat = new
            {
                SetTermostat = new Dictionary<string, object>()
            };
            modelThermostat.SetTermostat.Add("TermostatId", thermostatRCModel.Id);
            modelThermostat.SetTermostat.Add("TermostatState", thermostatRCModel.State);

            if (thermostatRCModel.State)
            {
                modelThermostat.SetTermostat.Add("Behavior", thermostatRCModel.Behavior);
                modelThermostat.SetTermostat.Add("TargetTemp", thermostatRCModel.TargetTemp);
            }
            this.Sender.SendMessageToReceiver(modelThermostat, thermostatRCModel.ReceiverIp);
        }

        public void SendThermostatDelete(ThermostatDeleteRCModel thermostatDeleteRCModel)
        {
            this.Sender.SendMessageToReceiver(new
            {
                SetDeleteThermostat = new
                {
                    TermostatId = thermostatDeleteRCModel.Id,
                }
            }, thermostatDeleteRCModel.ReceiverIp);
        }

        public void SendClearEEPROM(string ReceiverIp)
        {
            this.Sender.SendMessageToReceiver(new { ClearEEPROM = "" }, ReceiverIp);
        }
    }
}
