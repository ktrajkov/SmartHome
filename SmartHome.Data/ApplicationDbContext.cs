using Microsoft.AspNet.Identity.EntityFramework;
using SmartHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHome.Data
{
    public class ApplicationDbContext : IdentityDbContextWithCustomUser<ApplicationUser>
    {
        public ApplicationDbContext()
        {

        }
        public IDbSet<House> Houses { get; set; }

        public IDbSet<Floor> Floors { get; set; }

        public IDbSet<Room> Rooms { get; set; }

        public IDbSet<Device> Devices { get; set; }

        public IDbSet<Sensor> Sensors { get; set; }

        public IDbSet<Thermostat> Thermostats { get; set; }

        public IDbSet<Temp> Temps { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Room>()
             .HasOptional(u => u.Thermostat)
             .WithMany()
             .HasForeignKey(u => u.ThermostatId);

            modelBuilder.Entity<Room>()
           .HasOptional(u => u.Sensor)
           .WithMany()
           .HasForeignKey(u => u.SensorId);


            modelBuilder.Entity<Thermostat>()
                   .HasRequired(t => t.Room)
                   .WithMany()
                   .HasForeignKey(t => t.RoomId)
                   .WillCascadeOnDelete(false);

            modelBuilder.Entity<Thermostat>()
                  .HasRequired(t => t.Sensor)
                  .WithMany()
                  .HasForeignKey(t => t.SensorId)
                  .WillCascadeOnDelete(false);

            modelBuilder.Entity<Thermostat>()
                        .HasRequired(t => t.Device)
                        .WithMany()
                        .HasForeignKey(t => t.DeviceId)
                        .WillCascadeOnDelete(true);

            modelBuilder.Entity<Sensor>()
                        .HasRequired(t => t.Room)
                        .WithMany()
                        .HasForeignKey(t => t.RoomId)
                        .WillCascadeOnDelete(true);    
        }

    }
}