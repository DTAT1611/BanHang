using BanHang.Models;
using BanHang.Models.EF;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Areas.Admin.Controllers
{
    public class ShipController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        // GET: Admin/Ship
        public ActionResult Index()
        {
            return View(dbConect.Ships);
        }
        public PartialViewResult UserShip()
        {
            ViewBag.user = new SelectList(dbConect.Users.Where(x=>x.Role=="Shipper").ToList(), "Id", "FullName");
            return PartialView();
        }
        [HttpPost]
        public ActionResult UpdateTT(int id, int trangthai,string userid)
        {
            var item = dbConect.Ships.Find(id);
            if (item != null)
            {
                dbConect.Ships.Attach(item);
                item.StatusShip = trangthai;
                item.userid = userid;
                dbConect.Entry(item).Property(x => x.StatusShip).IsModified = true;
                dbConect.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }
        public ActionResult AddShip()
        {
            dbConect.Ships.Add(new Ship
            {
                StatusShip = 1,
                userid = null,
                CreatedDate=DateTime.Now,
                ModifierDate= DateTime.Now,
                CreatedBy=User.Identity.GetUserId(),
            });
            dbConect.SaveChanges();
            return Json(new { Success = true });
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = dbConect.Ships.Find(id);
            if (item != null)
            {
                dbConect.Ships.Remove(item);
                dbConect.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}