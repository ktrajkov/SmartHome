using SmartHome.Data;
using SmartHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.Areas.Admin.HelpersClass
{
    public static class HelperClass
    {
        /// <summary>
        /// If the current pin is passed, it will be added and will be back with other allowed pins
        /// </summary>
        /// <param name="data"></param>
        /// <param name="houseId"></param>
        /// <param name="currentPin"></param>
        /// <returns></returns>
        internal static IEnumerable<Int16> GetAllowedPins(IUowData data, int houseId, Int16? currentPin)
        {
            var house = data.Houses.All()
                .Where(h => h.Id == houseId)
                .Select(h => new
                {
                    MaxNumberPins = h.MaxNumberPins,
                }).SingleOrDefault();
            if (house == null)
            {
                throw new ArgumentException("No house with this id");
            }

            if (currentPin != null && currentPin > house.MaxNumberPins)
            {
                throw new ArgumentException("The current pin can not be greater than " + house.MaxNumberPins);
            }
            var pins = Enumerable.Range(0, house.MaxNumberPins).Select(s => (Int16)s);
            var usedPinDevices = data.Devices.All()
                     .Where(d => d.Room.Floor.HouseId == houseId)
                     .Select(s => s.AttachedPin);
            var usedPinSensors = data.Sensors.All()
                    .Where(d => d.Room.Floor.HouseId == houseId)
                    .Select(s => s.AttachedPin);
            var usedPin = usedPinDevices.Union(usedPinSensors);
            var unusedPin = pins.Except(usedPin);
            if (currentPin == null)
            {
                return unusedPin;
            }
            else
            {
                var allowedPin = new[] { currentPin.Value }.Concat(unusedPin);
                return allowedPin;
            }
        }

        /// <summary>
        ///    Verifies the number of sensors
        /// </summary>
        /// <param name="data"></param>
        /// <param name="roomId"></param>
        /// <returns>
        /// If number of sensors for this house has reached maximum value return false .
        /// </returns>
        internal static bool CheckArraySizeSensors(IUowData data, int houseId)
        {
            var house = data.Houses.All()
                 .Where(h => h.Id == houseId)
                 .Select(h => new
                 {
                     MaxArraySizeSensors = h.MaxArraySizeSensors,
                 }).SingleOrDefault();
            if (house == null)
            {
                throw new ArgumentException("No house with this id");
            }

            var countSensors = data.Sensors.All()
                .Where(s => s.Room.Floor.HouseId == houseId).Count();
            return !(house.MaxArraySizeSensors < countSensors + 1);
        }

        /// <summary>
        ///    Verifies the number of thermostats
        /// </summary>
        /// <param name="data"></param>
        /// <param name="roomId"></param>
        /// <returns>
        /// If number of thermostats for this house has reached maximum value return false .
        /// </returns>
        internal static bool CheckArraySizeThermostats(IUowData data, int houseId)
        {
            var house = data.Houses.All()
               .Where(h => h.Id == houseId)
               .Select(h => new
               {
                   MaxArraySizeTermostats = h.MaxArraySizeTermostats,
               }).SingleOrDefault();
            if (house == null)
            {
                throw new ArgumentException("No house with this id");
            }
            var countThermostats = data.Thermostats.All()
                .Where(s => s.Room.Floor.HouseId == houseId).Count();
            return !(house.MaxArraySizeTermostats < countThermostats + 1);
        }

        internal static SelectList GetAllUsers(IUowData data, string userId)
        {
            var users = data.Users.All()
                 .Select(u => new
                 {
                     Id = u.Id,
                     UserName = u.UserName
                 });
            return new SelectList(users, "Id", "UserName", userId);
        }

        internal static SelectList GetDevicesInRoom(IUowData data, int roomId, int? currentDeviceId)
        {
            var devices = data.Rooms.All()
                .Where(r => r.Id == roomId)
                .Select(r => r.Devices.Select(d => new
                {
                    Id = d.Id,
                    Name = d.Name
                })).SingleOrDefault();
            return new SelectList(devices, "Id", "Name", currentDeviceId);
        }
    }
}