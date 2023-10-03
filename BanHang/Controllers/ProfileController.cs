using BanHang.Models;
using Microsoft.AspNet.Identity;
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
        private ApplicationDbContext dbConect = new ApplicationDbContext();
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
                dbConect.Users.Attach(model);


                dbConect.Entry(model).State = EntityState.Modified;
                
                
                dbConect.SaveChanges();

                return RedirectToAction("Index","Profile",new {id=model.Id});

            }

            ViewBag.Role = new SelectList(dbConect.Roles.ToList(), "Name", "Name");
            return View(model);

        }
    }
}