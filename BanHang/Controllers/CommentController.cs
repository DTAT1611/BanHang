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
        public ActionResult Index(int id)
        {
            var items = dbConect.Comments.Where(x => x.Product.Id == id).ToList();
            
            return PartialView(items);
        }

    }
}