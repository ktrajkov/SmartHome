using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHome.Api.Infrastructure.Abstract;
using SmartHome.Api.Models;
using SmartHome.Api.Properties;
using SmartHome.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web.Http;



namespace SmartHome.Api.Controllers
{
    public class ArduinoController : ApiController
    {
        private IEmailSender sender;
        private IUowData data;
        public ArduinoController(IEmailSender sender, IUowData data)
        {
            this.sender = sender;
            this.data = data;
        }
        [HttpPost]
        [ActionName("PostAlarm")]
        public void PostAlarm(dynamic sensorJson)
        {
            Sensor alarmSensor = JsonConvert.DeserializeObject<Sensor>(sensorJson.ToString());
            var alarmModel = this.data.Sensors.All()
                .Where(s => s.ArduinoArraySensorsId == alarmSensor.ArduinoArraySensorsId)
                .Select(s => new
                {
                    Sensor=s,                   
                    CurrentTemp = alarmSensor.CurrentTemp,
                    RoomName = s.Room.Name,
                    FloorName = s.Room.Floor.Name,
                    HouseName = s.Room.Floor.House.Name,
                    Address = s.Room.Floor.House.Address,
                    OwnerEmail = s.Room.Floor.House.User.Email

                }).SingleOrDefault();

            if(alarmModel==null)
            {
                return;
            }
            alarmModel.Sensor.AlarmCheck = false;
            this.data.Sensors.Update(alarmModel.Sensor);
            this.data.SaveChanges();

            StringBuilder body = new StringBuilder();
            body.Append("House: " + alarmModel.HouseName + System.Environment.NewLine);
            body.Append("Address: " + alarmModel.Address + System.Environment.NewLine);
            body.Append("Floor: " + alarmModel.FloorName + System.Environment.NewLine);
            body.Append("Room: " + alarmModel.RoomName + System.Environment.NewLine);
            body.Append("Current Temp: " + alarmModel.CurrentTemp.ToString() + System.Environment.NewLine);
            body.Append("Min. Temp. : " + alarmModel.Sensor.MinTempAlert.ToString() + System.Environment.NewLine);
            body.Append("Max. Temp. : " + alarmModel.Sensor.MaxTempAlert.ToString() + System.Environment.NewLine);
            MailMessage message = new MailMessage
            (
                Settings.Default.EmailAdmin,    //from
                alarmModel.OwnerEmail,          //to
                Settings.Default.EmailSubject,  //subject
                body.ToString()                 //body
            );
            sender.SendEmail(message);
        }

        [HttpPost]
        [ActionName("PostSensors")]
        public void PostSensors(dynamic tempsJson)
        {
            if (tempsJson.Temps != null)
            {
                foreach (var temp in tempsJson.Temps)
                {
                    Sensor sensorModel = JsonConvert.DeserializeObject<Sensor>(temp.ToString());
                    var sensorFromDB = this.data.Sensors.All()
                        .Where(a => a.ArduinoArraySensorsId == sensorModel.ArduinoArraySensorsId)
                        .SingleOrDefault();
                    if (sensorFromDB == null)
                    {
                        return;
                    }
                    sensorFromDB.CurrentTemp = sensorModel.CurrentTemp;
                    this.data.Sensors.Update(sensorFromDB);
                    this.data.Temps.Add(new SmartHome.Models.Temp
                        {
                            DateTimeUpdate = DateTime.Now,
                            SensorId = sensorFromDB.Id,
                            Temperature = sensorModel.CurrentTemp
                        });
                    this.data.SaveChanges();
                }
            }

        }
    }
}
