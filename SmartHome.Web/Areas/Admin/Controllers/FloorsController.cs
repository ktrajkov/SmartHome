using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SmartHome.Models;
using SmartHome.Data;
using SmartHome.Web.Areas.Admin.Models;
using SmartHome.RemoteControl.Abstract;

namespace SmartHome.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FloorsController : BaseController
    {
        public FloorsController(IUowData data, IRemoteControl remoteControl) : base(data, remoteControl) { }

        public ActionResult List(int houseId)
        {

            var floorsViewModel = this.Data.Houses.All()
                .Where(h => h.Id == houseId)
                .Select(s => new FloorListViewModel
                {
                    HouseId = s.Id,
                    Floors = s.Floоrs.Select(f => new FloorViewModel
                    {
                        Id = f.Id,
                        Name = f.Name
                    })
                }).SingleOrDefault();
            if (floorsViewModel == null)
            {
                return HttpNotFound();
            }
            return View(floorsViewModel);
        }

        public ActionResult Create(int? houseId)
        {
            if (houseId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FloorCreateViewModel floorCreateViewModel = new FloorCreateViewModel
            {
                HouseId = houseId.Value
            };
            return View(floorCreateViewModel);

        }

        [HttpPost]
        public ActionResult Create(FloorCreateViewModel floorCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                Floor floor = new Floor
                {
                    HouseId = floorCreateViewModel.HouseId,
                    Name = floorCreateViewModel.Name
                };
                this.Data.Floors.Add(floor);
                this.Data.SaveChanges();
                return RedirectToAction("List", new { HouseId = floor.HouseId });
            }
            return View(floorCreateViewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var floor = this.Data.Floors.GetById(id.Value);
            if (floor == null)
            {
                return HttpNotFound();
            }
            FloorEditViewModel floorEditViewModel = new FloorEditViewModel
            {
                HouseId = floor.HouseId,
                Id = floor.Id,
                Name = floor.Name
            };
            return View(floorEditViewModel);
        }

        [HttpPost]
        public ActionResult Edit(FloorEditViewModel floorEditViewModel)
        {
            if (ModelState.IsValid)
            {
                Floor floor = this.Data.Floors.GetById(floorEditViewModel.Id);
                if (floor == null)
                {
                    return HttpNotFound();
                }
                floor.Name = floorEditViewModel.Name;
                this.Data.Floors.Update(floor);
                this.Data.SaveChanges();
                return RedirectToAction("List", "Floors", new { HouseId = floor.HouseId });
            }
            return View(floorEditViewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Floor floor = this.Data.Floors.GetById(id.Value);
            if (floor == null)
            {
                return HttpNotFound();
            }
            return View(floor);
        }


        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Floor floor = this.Data.Floors.GetById(id);
            if (floor == null)
            {
                return HttpNotFound();
            }
            this.Data.Floors.Delete(floor);
            this.Data.SaveChanges();
            return RedirectToAction("List", "Floors", new { HouseId = floor.HouseId });
        }


    }
}
