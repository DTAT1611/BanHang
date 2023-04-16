using BanHang.Models.EF;
using BanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Areas.Admin.Controllers
{
    public class ProductCategoryController : Controller
    {
        // GET: Admin/ProductCategory
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        // GET: Admin/ProductCategory
        public ActionResult Index()
        {

            var items = dbConect.ProductCategories;
            return View(items);
        }

        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ProductCategories model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.ModifierDate = DateTime.Now;
                model.Alias = BanHang.Models.Common.Filter.FilterChar(model.Tiltle);
                dbConect.ProductCategories.Add(model);
                dbConect.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            var item = dbConect.ProductCategories.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductCategories model)
        {
            if (ModelState.IsValid)
            {
                model.ModifierDate = DateTime.Now;
                model.Alias = BanHang.Models.Common.Filter.FilterChar(model.Tiltle);
                dbConect.ProductCategories.Attach(model);
                dbConect.Entry(model).State = System.Data.Entity.EntityState.Modified;
                dbConect.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = dbConect.ProductCategories.Find(id);
            if (item != null)
            {
                dbConect.ProductCategories.Remove(item);
                dbConect.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        //[HttpPost]
        //public ActionResult IsActive(int id)
        //{
        //    var item = dbConect.News.Find(id);
        //    if (item != null)
        //    {
        //        item.IsActive = !item.IsActive;
        //        dbConect.Entry(item).State = System.Data.Entity.EntityState.Modified;
        //        dbConect.SaveChanges();
        //        return Json(new { success = true, IsActive = item.IsActive });
        //    }
        //    return Json(new { success = false });
        //}
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
                        var obj = dbConect.ProductCategories.Find(Convert.ToInt32(item));
                        dbConect.ProductCategories.Remove(obj);
                        dbConect.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = true });
        }
    }
}