using BanHang.Models.EF;
using BanHang.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Areas.Admin.Controllers
{
    public class SaleController : Controller
    {
        // GET: Admin/Sale
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        // GET: Admin/Ship
        public ActionResult Index()
        {
            
            return View(dbConect.Sales);
        }
        public PartialViewResult UserSale()
        {
            ViewBag.user = new SelectList(dbConect.Users.Where(x => x.Role == "CUS").ToList(), "Id", "FullName");
            return PartialView();
        }
        [HttpPost]
        public ActionResult UpdateTT(int id,int percent, string userid)
        {
            var item = dbConect.Sales.Find(id);
            if (item != null)
            {
                dbConect.Sales.Attach(item);
                item.userid = userid;
                item.percent = percent;
                dbConect.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }
        public ActionResult AddSale()
        {
            dbConect.Sales.Add(new Sale
            {
                userid = null,
                percent=0,
                CreatedDate = DateTime.Now,
                ModifierDate = DateTime.Now,
                CreatedBy = User.Identity.GetUserId(),
            });
            dbConect.SaveChanges();
            return Json(new { Success = true });
        }
        
        public ActionResult Api()
        {
            return Json(new { Success = true });
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = dbConect.Sales.Find(id);
            
            if (item != null)
            {
                dbConect.Sales.Remove(item);
                dbConect.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}