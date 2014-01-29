namespace SmartHome.Data.Migrations
{
    using SmartHome.Data;
    using SmartHome.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //            context.Database.ExecuteSqlCommand(@"CREATE UNIQUE NONCLUSTERED INDEX uc_RoomThermostatId
            //                                                        ON Rooms(ThermostatId)WHERE ThermostatId IS NOT NULL;GO");
            //            context.Database.ExecuteSqlCommand(@"CREATE UNIQUE NONCLUSTERED INDEX uc_RoomSensorId 
            //                                                        ON Rooms(SensorId)WHERE SensorId IS NOT NULL;GO");

            //context.Database.ExecuteSqlCommand("ALTER TABLE Sensors ADD CONSTRAINT uc_SensorRoomId UNIQUE(RoomId)");
            //context.Database.ExecuteSqlCommand("ALTER TABLE Thermostats ADD CONSTRAINT uc_RoomId UNIQUE(RoomId)");
            //context.Database.ExecuteSqlCommand("ALTER TABLE Thermostats ADD CONSTRAINT uc_DeviceId UNIQUE(DeviceId)");
            //context.Database.ExecuteSqlCommand("ALTER TABLE Thermostats ADD CONSTRAINT uc_SensorId UNIQUE(SensorId)");
        }
    }
}

