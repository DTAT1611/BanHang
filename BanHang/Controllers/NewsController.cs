using BanHang.Models;
using BanHang.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BanHang.Controllers
{
   
    public class NewsController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        // GET: News
        public ActionResult Index(int? page)
        {
            var pageSize = 1;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<New> items = dbConect.News.OrderByDescending(x => x.CreatedDate);
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }
        public ActionResult Detail(int id)
        {
            var item = dbConect.News.Find(id);
            return View(item);
        }
        public ActionResult Partial_News_Home()
        {
            var items = dbConect.News.Take(3).ToList();
            return PartialView(items);
        }
    }
}