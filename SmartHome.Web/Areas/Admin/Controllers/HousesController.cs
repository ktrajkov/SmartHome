using SmartHome.Data;
using SmartHome.Models;
using SmartHome.RemoteControl.Abstract;
using SmartHome.RemoteControl.Models;
using SmartHome.Web.Areas.Admin.HelpersClass;
using SmartHome.Web.Areas.Admin.Models;
using SmartHome.Web.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HousesController : BaseController
    {
        public HousesController(IUowData data, IRemoteControl remoteControl) : base(data, remoteControl) { }
        public ViewResult List()
        {
            var houses = this.Data.Houses.All()
                .Select(h => new HouseListViewModel
                {
                    Id = h.Id,
                    Name = h.Name,
                    Adress = h.Address,
                    DateCreated = h.DateCreate,
                    Owner = h.User.UserName
                });
            return View(houses);
        }

        public ViewResult Create()
        {
            HouseCreateViewModel houseCreateViewModel = new HouseCreateViewModel
            {
                DateCreate = DateTime.Now,
                ServerIp = Settings.Default.ServerIp,
                ServerPort = Settings.Default.ServerPort,
                ReceiverIp = Settings.Default.ReceiverIp,
                TimeToCheckTemp = Settings.Default.TimeToCheckTemp,
                TimeToSendTemp = Settings.Default.TimeToSendTemp,
                MaxArraySizeSensors = Settings.Default.MaxArraySizeSensors,
                MaxArraySizeTermostats = Settings.Default.MaxArraySizeTermostats,
                MaxNumberPins = Settings.Default.MaxNumberPins,
                Users = HelperClass.GetAllUsers(this.Data, null)
            };
            return View(houseCreateViewModel);
        }

        [HttpPost]
        public ActionResult Create(HouseCreateViewModel houseCreateViewModel, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                House house = new House
               {
                   Address = houseCreateViewModel.Address,
                   ReceiverIp = houseCreateViewModel.ReceiverIp,
                   DateCreate = houseCreateViewModel.DateCreate,
                   MaxArraySizeSensors = houseCreateViewModel.MaxArraySizeSensors,
                   MaxArraySizeTermostats = houseCreateViewModel.MaxArraySizeTermostats,
                   MaxNumberPins = houseCreateViewModel.MaxNumberPins,
                   Name = houseCreateViewModel.Name,
                   SecretKey = houseCreateViewModel.SecretKey,
                   ServerIp = houseCreateViewModel.ServerIp,
                   ServerPort = houseCreateViewModel.ServerPort,
                   TimeToCheckTemp = houseCreateViewModel.TimeToCheckTemp,
                   TimeToSendTemp = houseCreateViewModel.TimeToSendTemp,
                   UserId = houseCreateViewModel.UserId
               };

                HouseRCModel houseRCModel = new HouseRCModel
                {
                    ServerIp = house.ServerIp,
                    ServerPort = house.ServerPort,
                    ReceiverIp = house.ReceiverIp,
                    TimeToCheckTemp = house.TimeToCheckTemp,
                    TimeToSendTemp = house.TimeToSendTemp,
                };
                if ((ImageUpload != null && ImageUpload.ContentLength > 0))
                {
                    var fileName = Path.GetFileName(ImageUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~" + Settings.Default.HousesPathImages), fileName);
                    ImageUpload.SaveAs(path);
                    house.ImageUrl = Settings.Default.HousesPathImages + fileName;
                }
                else
                {
                    house.ImageUrl = Settings.Default.HousesPathImages + Settings.Default.DefaultHouseImage;
                }

                using (TransactionScope transaction = new TransactionScope())
                {
                    this.Data.Houses.Add(house);
                    this.Data.SaveChanges();

                    this.RemoteControl.SendHouseSettings(houseRCModel);
                    transaction.Complete();
                }

                return RedirectToAction("List");
            }

            houseCreateViewModel.Users = HelperClass.GetAllUsers(this.Data, null);
            return View(houseCreateViewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = this.Data.Houses.GetById(id.Value);
            if (house == null)
            {
                return HttpNotFound();
            }
            HouseEditViewModel houseEditViewModel = new HouseEditViewModel
            {
                Id = house.Id,
                Address = house.Address,
                ReceiverIp = house.ReceiverIp,
                DateCreate = house.DateCreate,
                MaxArraySizeSensors = house.MaxArraySizeSensors,
                MaxArraySizeTermostats = house.MaxArraySizeTermostats,
                MaxNumberPins = house.MaxNumberPins,
                Name = house.Name,
                SecretKey = house.SecretKey,
                ServerIp = house.ServerIp,
                ServerPort = house.ServerPort,
                TimeToCheckTemp = house.TimeToCheckTemp,
                TimeToSendTemp = house.TimeToSendTemp,
                Users = HelperClass.GetAllUsers(this.Data, null),
                UserId = house.UserId

            };
            return View(houseEditViewModel);
        }
        [HttpPost]
        public ActionResult Edit(HouseEditViewModel houseEditViewModel, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                House house = this.Data.Houses.GetById(houseEditViewModel.Id);
                if (house == null)
                {
                    return HttpNotFound();
                }
                house.Address = houseEditViewModel.Address;
                house.ReceiverIp = houseEditViewModel.ReceiverIp;
                house.DateCreate = houseEditViewModel.DateCreate;
                house.MaxArraySizeSensors = houseEditViewModel.MaxArraySizeSensors;
                house.MaxArraySizeTermostats = houseEditViewModel.MaxArraySizeTermostats;
                house.MaxNumberPins = houseEditViewModel.MaxNumberPins;
                house.Name = houseEditViewModel.Name;
                house.SecretKey = houseEditViewModel.SecretKey;
                house.ServerIp = houseEditViewModel.ServerIp;
                house.ServerPort = houseEditViewModel.ServerPort;
                house.TimeToCheckTemp = houseEditViewModel.TimeToCheckTemp;
                house.TimeToSendTemp = houseEditViewModel.TimeToSendTemp;
                house.UserId = houseEditViewModel.UserId;

                HouseRCModel houseRCModel = new HouseRCModel
                {
                    ServerIp = house.ServerIp,
                    ServerPort = house.ServerPort,
                    ReceiverIp = house.ReceiverIp,
                    TimeToCheckTemp = house.TimeToCheckTemp,
                    TimeToSendTemp = house.TimeToSendTemp,
                };

                if ((ImageUpload != null && ImageUpload.ContentLength > 0))
                {
                    var fileName = Path.GetFileName(ImageUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~" + Settings.Default.HousesPathImages), fileName);
                    ImageUpload.SaveAs(path);
                    house.ImageUrl = Settings.Default.HousesPathImages + fileName;
                }
                else
                {
                    house.ImageUrl = Settings.Default.HousesPathImages + Settings.Default.DefaultHouseImage;
                }
                using (TransactionScope transaction = new TransactionScope())
                {
                    this.Data.Houses.Update(house);
                    this.Data.SaveChanges();

                    this.RemoteControl.SendHouseSettings(houseRCModel);
                    transaction.Complete();
                }


                return RedirectToAction("List");
            }
            houseEditViewModel.Users = HelperClass.GetAllUsers(this.Data, houseEditViewModel.UserId);
            return View(houseEditViewModel);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var houseDeleteViewModel = this.Data.Houses.All()
                .Where(h => h.Id == id)
                .Select(s => new HouseDeleteViewModel
                {
                    Name = s.Name,
                    Address = s.Address,
                    DateCreate = s.DateCreate
                }).SingleOrDefault();
            if (houseDeleteViewModel == null)
            {
                return HttpNotFound();
            }
            return View(houseDeleteViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id) 
        {
            var houseReceiver = this.Data.Houses.All()
                 .Where(h => h.Id == id)
                 .Select(r=>r.ReceiverIp)
                 .SingleOrDefault();
            if (houseReceiver == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (houseReceiver == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (TransactionScope transaction = new TransactionScope())
            {
                this.Data.Houses.Delete(id);
                this.Data.SaveChanges();

                this.RemoteControl.SendClearEEPROM(houseReceiver);
                transaction.Complete();
            }

            return RedirectToAction("List");
        }

        public ActionResult ClearEEPROM(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseClearEEPROMVIewModel houseClearEEPROMViewModel = this.Data.Houses.All()
                .Where(h => h.Id == id)
                .Select(s => new HouseClearEEPROMVIewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .SingleOrDefault();
            if (houseClearEEPROMViewModel == null)
            {
                return HttpNotFound();
            }
            return View(houseClearEEPROMViewModel);
        }

        [HttpPost, ActionName("ClearEEPROM")]
        public ActionResult ClearEEPROMConfirmed(int id)
        {
            var houseReceiver = this.Data.Houses.All()
                  .Where(h => h.Id == id)
                  .Select(h => h.ReceiverIp)
                  .SingleOrDefault();
            if (houseReceiver == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            this.RemoteControl.SendClearEEPROM(houseReceiver);

            return RedirectToAction("List");
        }

    }
}