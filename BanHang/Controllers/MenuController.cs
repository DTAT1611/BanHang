using BanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Controllers
{
    public class MenuController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MenuTop()
        {
            var items=dbConect.Categories.OrderBy(x=>x.Position).ToList();
            return PartialView("_MenuTop",items);
        }
        public ActionResult MenuProductCategory()
        {
            var items=dbConect.ProductCategories.ToList();
            return PartialView("_MenuProductCategory", items);
        }
        public ActionResult MenuArrivals()
        {
            var items = dbConect.ProductCategories.ToList();
            return PartialView("_MenuArrivals", items);
        }
        public ActionResult BestSeller()
        {
            var items = dbConect.ProductCategories.ToList();
            return PartialView("_BestSeller", items);
        }
    }
}