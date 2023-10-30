using BanHang.Models;
using BanHang.Models.EF;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BanHang.Controllers
{
    public class CommentController : Controller
    {

        private ApplicationDbContext dbConect = new ApplicationDbContext();
       
        // GET: Comment
        public PartialViewResult Index(int id)
        {
            var items = dbConect.Comments.Where(x => x.Product.Id == id).ToList();
            
            return PartialView(items);
        }
        //////public PartialViewResult Create(int id)
        //////{

        //////    return PartialView("Create");
        //////}
        
        public ActionResult Edit(int id)
        {
            return View(dbConect.Comments.Find(id));
        }
        [HttpPost]
        public ActionResult Edit(Comment model)
        {
            if (ModelState.IsValid)
            {
                model.ModifierDate = DateTime.Now;
                dbConect.Comments.Attach(model);
                dbConect.SaveChanges();
               
            }
            return View(model);
        }

    }
}