using BanHang.Models;
using BanHang.Models.EF;
using Microsoft.AspNet.Identity;
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
            var items=dbConect.Categories.OrderBy(x=>x.Position).Where(x=>x.IsActive).ToList();
            return PartialView("_MenuTop",items);
        }
        public ActionResult MenuProductCategory()
        {
            var items=dbConect.ProductCategories.ToList();
            return PartialView("_MenuProductCategory", items);
        }
        public ActionResult MenuArrivals()
        {
            var currentuser = User.Identity.GetUserId();
            var SaleItems = dbConect.Sales.Where(x => x.userid == currentuser).ToList();
            List<Product> items = new List<Product>();
            foreach(var item in SaleItems)
            {
                if (item.productid > 0)
                {
                    items.Add(dbConect.Products.Where(x => x.Id == item.productid).FirstOrDefault());
                }
               
            }
            return PartialView("_MenuArrivals", items);
        }
        public ActionResult BestSeller()
        {
            var items = dbConect.ProductCategories.ToList();
            return PartialView("_BestSeller", items);
        }
        public ActionResult MenuLeft(int? id)
        {
            if (id != null)
            {
                ViewBag.CateId = id;
            }
            var items = dbConect.ProductCategories.ToList();
            return PartialView("_MenuLeft", items);
        }
        public ActionResult ProductsIndex()
        {
            var items = dbConect.ProductCategories.ToList();
            return PartialView("ProductsIndex", items);
        }

    }
}