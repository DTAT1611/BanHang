using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanHang.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BanHang.Areas.Admin.Controllers
{
    //  [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        // GET: Admin/Role
        public ActionResult Index()
        {
            var items = dbConect.Roles.ToList();

            return View(items);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                var rolemanager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbConect));
                rolemanager.Create(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            var item = dbConect.Roles.Find(id);
            if (item != null)
            {
                dbConect.Roles.Remove(item);
                dbConect.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}