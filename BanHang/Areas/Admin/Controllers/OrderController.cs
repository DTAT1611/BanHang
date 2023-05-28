using BanHang.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace BanHang.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    public class OrderController : Controller
    {
        // GET: Admin/Order
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        public ActionResult Index()
        {
            var items = dbConect.Orders;
            return View(items);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = dbConect.Orders.Find(id);
            if (item != null)
            {
                //var DeleteItem=dbConect.Categories.Attach(item);
                dbConect.Orders.Remove(item);
                dbConect.SaveChanges();
                return Json(new { success = true });
            }
            return Json(false);
        }
        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = dbConect.Orders.Find(Convert.ToInt32(item));
                        dbConect.Orders.Remove(obj);
                        dbConect.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = true });
        }
        public ActionResult Details(int id)
        {
            var items = dbConect.OrderDetails.Where(x=>x.OrderId==id);
            return View(items);
        }
        public ActionResult DeleteDetail(int id)
        {
            var item = dbConect.OrderDetails.Find(id);
            if (item != null)
            {
                //var DeleteItem=dbConect.Categories.Attach(item);
                dbConect.OrderDetails.Remove(item);
                dbConect.SaveChanges();
                return Json(new { success = true });
            }
            return Json(false);
        }
        public ActionResult GetShip(int id) {
            
            dbConect.Orders.Find(id).ShipperID = User.Identity.GetUserId();
            dbConect.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult DeleteAllDetails(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = dbConect.OrderDetails.Find(Convert.ToInt32(item));
                        dbConect.OrderDetails.Remove(obj);
                        dbConect.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = true });
        }
    }
}