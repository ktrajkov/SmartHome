using SmartHome.Data;
using SmartHome.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Data
{
    public class UowData:IUowData
    {
        private readonly DbContext context;
        private readonly Dictionary<Type, object> repositories = new Dictionary<Type, object>();      
        public UowData(DbContext context)
        {
            this.context = context;
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            if (!this.repositories.ContainsKey(typeof(T)))
            {
                var type = typeof(GenericRepository<T>);               

                this.repositories.Add(typeof(T), Activator.CreateInstance(type, this.context));
            }

            return (IRepository<T>)this.repositories[typeof(T)];
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public void Dispose()
        {
            this.context.Dispose();
        }

        public IRepository<House> Houses
        {
            get { return this.GetRepository<House>(); }
        }

        public IRepository<Floor> Floors
        {
            get { return this.GetRepository<Floor>(); }
        }

        public IRepository<Room> Rooms
        {
            get { return this.GetRepository<Room>(); }
        }

        public IRepository<Device> Devices
        {
            get { return this.GetRepository<Device>(); }
        }

        public IRepository<Sensor> Sensors
        {
            get { return this.GetRepository<Sensor>(); }
        }

        public IRepository<Temp> Temps
        {
            get { return this.GetRepository<Temp>(); }
        }

        public IRepository<Thermostat> Thermostats
        {
            get { return this.GetRepository<Thermostat>(); }
        }            

        public IRepository<ApplicationUser> Users
        {
            get { return this.GetRepository<ApplicationUser>(); }
        }
    }
}
