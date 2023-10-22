using BanHang.Models;
using BanHang.Models.EF;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public PartialViewResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult Create(Comment model,int id)
        {
            model.Product = dbConect.Products.Find(id);
            model.ApplicationUsers = dbConect.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                model.Reply = 0;
                model.CreatedDate=DateTime.Now;
                model.ModifierDate=DateTime.Now;
                dbConect.Comments.Add(model);
                dbConect.SaveChanges();
                
            }
            return PartialView("Create",model);
        }

    }
}