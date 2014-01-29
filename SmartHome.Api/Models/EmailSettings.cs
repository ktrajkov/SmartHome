using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Api.Models
{
    public class EmailSettings
    {
        public bool UseSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ServerName { get; set; }
        public int ServerPort { get; set; }      
    }
}