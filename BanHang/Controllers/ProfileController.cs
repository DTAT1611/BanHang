using BanHang.Models;
using BanHang.Models.EF;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Controllers
{
    public class ProfileController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        public ProfileController()
        {

        }
        public ProfileController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Profile
        public ActionResult Index(string id)
        {
            ViewBag.Role = new SelectList(dbConect.Roles.ToList(), "Name", "Name");
            var item = dbConect.Users.Find(id);
            return View(item);
        }
        public ActionResult Edit(string id)
        {
            ViewBag.Role = new SelectList(dbConect.Roles.ToList(), "Name", "Name");
            var item = dbConect.Users.Find(id);
            return View(item);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                
                var oldUser = UserManager.FindById(model.Id);
                var oldRoleId = oldUser.Roles.SingleOrDefault().RoleId;
                var oldRoleName = dbConect.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;
                UserManager.AddToRole(model.Id, oldRoleName);
                dbConect.Users.Attach(model);
                dbConect.Users.Find(model.Id).EmailConfirmed = true;
                dbConect.Entry(model).State = EntityState.Modified;
                

                dbConect.SaveChanges();
               
                return RedirectToAction("Index","Profile",new {id=model.Id});

            }

            ViewBag.Role = new SelectList(dbConect.Roles.ToList(), "Name", "Name");
            return View(model);

        }
        public PartialViewResult Ship(string id)
        {
            return PartialView(dbConect.Ships.Where(x=>x.userid==id));
        }
        public PartialViewResult Order(int id)
        {
            
            return PartialView(dbConect.Orders.Where(x => x.idship == id).ToList());
        }
        public PartialViewResult Orderd(int id)
        {

            return PartialView(dbConect.OrderDetails.Where(x => x.OrderId == id).ToList());
        }
        public PartialViewResult NameProduct(int id)
        {
            var n = dbConect.Products.Find(id);
            return PartialView(n);
        }
        public ActionResult Shipped(int id, int trangthai)
        {
            var item = dbConect.Ships.Find(id);
            if (item != null)
            {
                dbConect.Ships.Attach(item);
                item.StatusShip = trangthai;
                dbConect.Entry(item).Property(x => x.StatusShip).IsModified = true;
                dbConect.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }
        public ActionResult UpdateTT(int id,int trangthai)
        {
            var item=dbConect.Orders.Find(id);
            if (item != null)
            {
                dbConect.Orders.Attach(item);
                item.Status = trangthai;
                dbConect.Entry(item).Property(x => x.Status).IsModified = true;
                dbConect.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }
        public ActionResult Cancle(int id, int trangthai)
        {
            var item = dbConect.Orders.Find(id);
            if (item != null)
            {
                dbConect.Orders.Attach(item);
                item.Status = trangthai;
                dbConect.Entry(item).Property(x => x.Status).IsModified = true;
                dbConect.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }
        public PartialViewResult OrderHistory(string id)
        {
            return PartialView(dbConect.Orders.Where(x=>x.ApplicationUsers.Id==id).ToList());
        }
    }
}