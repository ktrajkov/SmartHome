using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Api.Models
{
    public class Sensor
    {          
        public float CurrentTemp { get; set; }
        public int ArduinoArraySensorsId { get; set; }    
    }
}