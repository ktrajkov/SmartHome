using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SmartHome.Models;
using SmartHome.Data;
using SmartHome.Web.Areas.Admin.Models;
using SmartHome.Web.Areas.Admin.HelpersClass;
using SmartHome.RemoteControl.Models;
using System.Transactions;
using SmartHome.RemoteControl.Abstract;

namespace SmartHome.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DevicesController : BaseController
    {
        public DevicesController(IUowData data, IRemoteControl remoteControl) : base(data, remoteControl) { }
        public ActionResult Create(int roomId)
        {
            var houseId = this.Data.Rooms.All()
              .Where(r => r.Id == roomId)
              .Select(r => r.Floor.HouseId)
              .SingleOrDefault();
            if (houseId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var allowedPin = HelperClass.GetAllowedPins(this.Data, houseId, null);
            if (!(allowedPin.Count() > 0))
            {
                return PartialView("_ErrorMessagePartial", "Тhere are no free pins");
            }
            DeviceCreateViewModel deviceCreateViewModel = new DeviceCreateViewModel
            {
                AllowedPins = new SelectList(allowedPin),
                RoomId = roomId
            };
            return View(deviceCreateViewModel);
        }

        [HttpPost]
        public ActionResult Create(DeviceCreateViewModel deviceCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                Device device = new Device
                {
                    AttachedPin = deviceCreateViewModel.AttachedPin,
                    Name = deviceCreateViewModel.Name,
                    RoomId = deviceCreateViewModel.RoomId,
                    State = deviceCreateViewModel.State
                };

                string ReceiverIp = this.Data.Rooms.All()
                    .Where(r => r.Id == deviceCreateViewModel.RoomId)
                    .Select(s => s.Floor.House.ReceiverIp)
                    .SingleOrDefault();
                if (ReceiverIp == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DeviceRCModel deviceRCModel = new DeviceRCModel
                {
                    ReceiverIp = ReceiverIp,
                    Pin = deviceCreateViewModel.AttachedPin,
                    State = deviceCreateViewModel.State,
                };

                using (TransactionScope transaction = new TransactionScope())
                {
                    this.Data.Devices.Add(device);
                    this.Data.SaveChanges();
                    this.RemoteControl.SendDeviceSettings(deviceRCModel);
                    transaction.Complete();
                }

                return RedirectToAction("RoomDetails", "Rooms", new { RoomId = device.RoomId });
            }
            return View(deviceCreateViewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var device = this.Data.Devices.All()
              .Where(d => d.Id == id.Value)
              .Select(d => new
              {
                  AttachedPin = d.AttachedPin,
                  Id = d.Id,
                  Name = d.Name,
                  RoomId = d.RoomId,
                  State = d.State,
                  HouseId = d.Room.Floor.HouseId

              })
              .SingleOrDefault();
            if (device == null)
            {
                return HttpNotFound();
            }
            var allowedPin = HelperClass.GetAllowedPins(this.Data, device.HouseId, device.AttachedPin);
            DeviceEditViewModel deviceEditViewModel = new DeviceEditViewModel
            {
                AttachedPin = device.AttachedPin,
                Id = device.Id,
                Name = device.Name,
                RoomId = device.RoomId,
                State = device.State,
                AllowedPins = new SelectList(allowedPin, device.AttachedPin)

            };
            return View(deviceEditViewModel);
        }

        [HttpPost]
        public ActionResult Edit(DeviceEditViewModel deviceEditViewModel)
        {
            if (ModelState.IsValid)
            {
                var editModel = this.Data.Devices.All()
                    .Where(d => d.Id == deviceEditViewModel.Id)
                    .Select(d => new
                {
                    Device = d,
                    ReceiverIp = d.Room.Floor.House.ReceiverIp
                }).SingleOrDefault();
                if (editModel == null)
                {
                    return HttpNotFound();
                }
                Int16? attachedPinForDeleting = null;
                if (editModel.Device.AttachedPin != deviceEditViewModel.AttachedPin)
                {
                    attachedPinForDeleting = editModel.Device.AttachedPin;
                }
                editModel.Device.AttachedPin = deviceEditViewModel.AttachedPin;
                editModel.Device.Name = deviceEditViewModel.Name;
                editModel.Device.State = deviceEditViewModel.State;

                using (TransactionScope transaction = new TransactionScope())
                {
                    this.Data.Devices.Update(editModel.Device);
                    this.Data.SaveChanges();

                    if (attachedPinForDeleting != null)
                    {
                        this.RemoteControl.SendDeviceDelete(
                            new DeviceDeleteRCModel
                        {
                            ReceiverIp = editModel.ReceiverIp,
                            AttachedPin = attachedPinForDeleting.Value
                        });
                    }

                    this.RemoteControl.SendDeviceSettings(
                        new DeviceRCModel
                        {
                            ReceiverIp = editModel.ReceiverIp,
                            Pin = deviceEditViewModel.AttachedPin,
                            State = deviceEditViewModel.State
                        });
                    transaction.Complete();
                }

                return RedirectToAction("RoomDetails", "Rooms", new { RoomId = deviceEditViewModel.RoomId });
            }
            return View(deviceEditViewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = this.Data.Devices.GetById(id.Value);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var deleteModel = this.Data.Devices.All()
               .Where(d => d.Id == id)
               .Select(s => new
               {
                   ReceiverIp = s.Room.Floor.House.ReceiverIp,
                   RoomId = s.RoomId,
                   DeviceId = s.Id,
                   AttachedPin = s.AttachedPin
               })
               .SingleOrDefault();
            if (deleteModel == null)
            {
                return HttpNotFound();
            }

            using (TransactionScope transaction = new TransactionScope())
            {
                this.Data.Devices.Delete(deleteModel.DeviceId);
                this.Data.SaveChanges();

                this.RemoteControl.SendDeviceDelete(
                     new DeviceDeleteRCModel
                     {
                         AttachedPin = deleteModel.AttachedPin,
                         ReceiverIp = deleteModel.ReceiverIp,
                     });
                transaction.Complete();
            }
            return RedirectToAction("RoomDetails", "Rooms", new { RoomId = deleteModel.RoomId });
        }

    }
}
