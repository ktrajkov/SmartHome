using SmartHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Data
{
    public interface IUowData : IDisposable
    {
       
        IRepository<House> Houses { get; }

        IRepository<Floor> Floors { get; }
        IRepository<Room> Rooms { get; }

        IRepository<Device> Devices { get; }

        IRepository<Sensor> Sensors { get; }

        IRepository<Thermostat> Thermostats { get; }

        IRepository<Temp> Temps { get; }

        IRepository<ApplicationUser> Users { get; }

        int SaveChanges();
    }
}
