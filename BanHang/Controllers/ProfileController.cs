using BanHang.Models;
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
    }
}