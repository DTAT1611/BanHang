using BanHang.Models;
using BanHang.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Areas.Admin.Controllers
{
   // [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
       private ApplicationDbContext dbConect = new ApplicationDbContext();
        // GET: Admin/Category
        public ActionResult Index()
        {
            var items = dbConect.Categories;
            return View(items);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Category model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.ModifierDate = DateTime.Now;
                model.Alias = BanHang.Models.Common.Filter.FilterChar(model.Tiltle);
                dbConect.Categories.Add(model);
                dbConect.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            var item = dbConect.Categories.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                dbConect.Categories.Attach(model);
                model.ModifierDate = DateTime.Now;
                model.Alias = BanHang.Models.Common.Filter.FilterChar(model.Tiltle);
                dbConect.Entry(model).Property(x => x.Alias).IsModified = true;
                dbConect.Entry(model).Property(x => x.Discription).IsModified = true;
                dbConect.Entry(model).Property(x => x.Tiltle).IsModified = true;
                dbConect.Entry(model).Property(x => x.SeoDiscription).IsModified = true;
                dbConect.Entry(model).Property(x => x.SeoKeywords).IsModified = true;
                dbConect.Entry(model).Property(x => x.SeoTitle).IsModified = true;
                dbConect.Entry(model).Property(x => x.Position).IsModified = true;
                dbConect.Entry(model).Property(x => x.ModifierDate).IsModified = true;
                dbConect.Entry(model).Property(x => x.ModifierBy).IsModified = true;
                dbConect.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item=dbConect.Categories.Find(id);
            if (item != null)
            {
                //var DeleteItem=dbConect.Categories.Attach(item);
                dbConect.Categories.Remove(item);
                dbConect.SaveChanges();
                return Json(new {success= true});
            }
            return Json(false);
        }
        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = dbConect.Categories.Find(id);
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
                        var obj = dbConect.Categories.Find(Convert.ToInt32(item));
                        dbConect.Categories.Remove(obj);
                        dbConect.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = true });
        }
    }
}