using BanHang.Models.EF;
using BanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
namespace BanHang.Areas.Admin.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        // GET: Admin/News
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<New> item = dbConect.News.OrderByDescending(x => x.Id);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                item = item.Where(x => x.Alias.Contains(Searchtext) || x.Title.Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            item = item.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(item);
        }
        public ActionResult Add()
        {
            ViewBag.Category = new SelectList(dbConect.Categories.ToList(), "Id", "Tiltle");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(New model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.ModifierDate = DateTime.Now;
                model.Alias = BanHang.Models.Common.Filter.FilterChar(model.Title);
                dbConect.News.Add(model);
                dbConect.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category = new SelectList(dbConect.Categories.ToList(), "Id", "Tiltle");
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.Category = new SelectList(dbConect.Categories.ToList(), "Id", "Tiltle");
            var item = dbConect.News.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(New model)
        {
            if (ModelState.IsValid)
            {
                model.ModifierDate = DateTime.Now;
                model.Alias = BanHang.Models.Common.Filter.FilterChar(model.Title);
                dbConect.News.Attach(model);
                dbConect.Entry(model).State = System.Data.Entity.EntityState.Modified;
                dbConect.SaveChanges();
                return RedirectToAction("Index");
            }
           
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = dbConect.News.Find(id);
            if (item != null)
            {
                dbConect.News.Remove(item);
                dbConect.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = dbConect.News.Find(id);
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
                        var obj = dbConect.News.Find(Convert.ToInt32(item));
                        dbConect.News.Remove(obj);
                        dbConect.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = true });
        }
    }
}