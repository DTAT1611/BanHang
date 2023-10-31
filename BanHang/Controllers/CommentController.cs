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
            ViewBag.proid = id;
            return PartialView(items);
        }
        public PartialViewResult Create(int id)
        {
            ViewBag.ID = id;
            return PartialView("Create");
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]

        public ActionResult AddCommnet(int productid, string com)
        {
            dbConect.Comments.Add(new Comment
            {
                Product = dbConect.Products.Find(productid),
                comms = com,
                ApplicationUsers = dbConect.Users.Find(User.Identity.GetUserId()),
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity.GetUserId(),
                ModifierDate = DateTime.Now,

            });
            
            dbConect.SaveChanges();

            return Json(new { Success = true });

        }
       
        [HttpPost]
        public ActionResult EditComment(int id,string com)
        {
            Comment c = dbConect.Comments.Find(id);
            if(c==null)
            {
                return HttpNotFound();
            }
            else
            {
                c.comms = com;
                dbConect.Comments.Attach(c);
                dbConect.Entry(c).State = System.Data.Entity.EntityState.Modified;
            }
            dbConect.SaveChanges();
            return Json(new { Success = true });
        }
        [HttpPost]
        public ActionResult DeleteComment(int id)
        {
            var item = dbConect.Comments.Find(id);
            if (item != null)
            {
                dbConect.Comments.Remove(item);
                dbConect.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        public PartialViewResult IndexReply(int id)
        {
            var item = dbConect.Comments.Where(x => x.Reply == id).ToList();
            return PartialView(item);
        }
        public PartialViewResult Reply(int id)
        {
            return PartialView("Reply");
        }
        [HttpPost]
        public ActionResult ReplyComment(int id,int productid,string com)
        {
            dbConect.Comments.Add(new Comment
            {
                Reply = id,
                Product = dbConect.Products.Find(productid),
                comms = com,
                ApplicationUsers = dbConect.Users.Find(User.Identity.GetUserId()),
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity.GetUserId(),
                ModifierDate = DateTime.Now,

            }); ;

            dbConect.SaveChanges();

            return Json(new { Success = true });
        }

    }
}