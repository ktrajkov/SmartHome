using SmartHome.RemoteControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.RemoteControl.Abstract
{
    public interface IRemoteControl
    {
        void SendSensorSettings(SensorRCModel sensorRCModel);

        void SendSensorSettings(SensorUserEditRCModel sensorRCModel);

        void SendSensorDelete(SensorDeleteRCModel sensorDeleteRCModel);

        void SendDeviceSettings(DeviceRCModel deviceRCModel);

        void SendDevicesSettings(DevicesRCModel deviceRCModel);

        void SendDeviceDelete(DeviceDeleteRCModel deviceDeleteRCModel);

        void SendHouseSettings(HouseRCModel houseRCModel);

        void SendHouseSettings(HouseUserEditRCModel houseRCModel);

        void SendThermostatSettings(ThermostatRCModel thermostatRCModel);

        void SendThermostatSettings(ThermostatUserEditRCModel thermostatRCModel);

        void SendThermostatDelete(ThermostatDeleteRCModel thermostatDeleteRCModel);

        void SendClearEEPROM(string ReceiverIp);

    }
}
