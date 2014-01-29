using Microsoft.AspNet.Identity;
using SmartHome.Data;
using SmartHome.Models;
using SmartHome.RemoteControl.Abstract;
using SmartHome.Web.HelpersClass;
using SmartHome.Web.Models;
using SmartHome.Web.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SmartHome.Web.Controllers
{
    [Authorize]
    public class TempStatisticsController : BaseController
    {
        public TempStatisticsController(IUowData data, IRemoteControl remoteControl) : base(data, remoteControl) { }

        [HttpGet]
        public ActionResult Index(int sensorId)
        {
            string userId = User.Identity.GetUserId();
            var temps = Data.Sensors.All()
             .Where(s => s.Id == sensorId && s.Room.Floor.House.UserId == userId)
             .Select(t => new
                {
                    DateTimeUpdate = t.Temps
                    .OrderByDescending(d => d.DateTimeUpdate)
                    .Select(d => d.DateTimeUpdate)
                    .Take(Settings.Default.NumberOfRecentTemp)
                }).SingleOrDefault();
            if (temps!=null && temps.DateTimeUpdate.Count() > 0)
            {
                DateRangeViewModel dateRangeViewModel = new DateRangeViewModel
                {
                    FromDate = temps.DateTimeUpdate.LastOrDefault(),
                    ToDate = temps.DateTimeUpdate.FirstOrDefault(),
                    SensorId = sensorId,
                };
                return PartialView(dateRangeViewModel);
            }
            return Content("No information for this sensor");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetTempStatistics(DateRangeViewModel dateRangeViewModel)
        {
            if (ModelState.IsValid)
            {
                dateRangeViewModel.ToDate = dateRangeViewModel.ToDate.AddDays(1);
                var temps = Data.Temps.All()
               .Where(s => s.SensorId == dateRangeViewModel.SensorId &&
                   s.DateTimeUpdate >= dateRangeViewModel.FromDate &&
                   s.DateTimeUpdate <= dateRangeViewModel.ToDate)
                   .Select(s => new
                   {
                       Datetime = s.DateTimeUpdate,
                       Temperature = s.Temperature
                   }).ToArray()
                   .Select(s => new object[] { s.Datetime.ToJavascriptTimestamp(), s.Temperature });
                if (temps.Count() > 0)
                {
                    return Json(temps, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new HttpException(500, "No information");                 
                }
            }
            else
            {
                 throw new HttpException(500, "The Range of the date is invalid");               
            }

        }
    }
}