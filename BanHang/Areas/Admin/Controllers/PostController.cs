using BanHang.Models.EF;
using BanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Areas.Admin.Controllers
{
    public class PostController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        // GET: Admin/Post
        public ActionResult Index()
        {
            var item = dbConect.Posts.ToList();
            return View(item);
        }
        public ActionResult Add()
        {
            ViewBag.Category = new SelectList(dbConect.Categories.ToList(), "Id", "Tiltle");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Post model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.CategoryID = 1;
                model.ModifierDate = DateTime.Now;
                model.Alias = BanHang.Models.Common.Filter.FilterChar(model.Title);
                dbConect.Posts.Add(model);
                dbConect.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category = new SelectList(dbConect.Categories.ToList(), "Id", "Tiltle");
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            ViewBag.Category = new SelectList(dbConect.Categories.ToList(), "Id", "Tiltle");
            var item = dbConect.Posts.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Post model)
        {
            if (ModelState.IsValid)
            {
                model.ModifierDate = DateTime.Now;
                model.Alias = BanHang.Models.Common.Filter.FilterChar(model.Title);
                dbConect.Posts.Attach(model);
                dbConect.Entry(model).State = System.Data.Entity.EntityState.Modified;
                dbConect.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = dbConect.Posts.Find(id);
            if (item != null)
            {
                dbConect.Posts.Remove(item);
                dbConect.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = dbConect.Posts.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                dbConect.Entry(item).State = System.Data.Entity.EntityState.Modified;
                dbConect.SaveChanges();
                return Json(new { success = true, IsActive = item.IsActive });
            }
            return Json(new { success = false });
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
                        var obj = dbConect.Posts.Find(Convert.ToInt32(item));
                        dbConect.Posts.Remove(obj);
                        dbConect.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = true });
        }
    }
}