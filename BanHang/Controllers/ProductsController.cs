using BanHang.Models;
using BanHang.Models.EF;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BanHang.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        public ActionResult Index(int? id)
        {
            var items = dbConect.Products.Where(x => x.IsActive).ToList();
            if (id > 0)
            {
                items = items.Where(x => x.ProductCategoryId == id).ToList();
            }
            if (id != null)
            {
                ViewBag.ProductCategoryAlias = dbConect.ProductCategories.Find(id).Alias;
                items = dbConect.Products.Where(x => x.IsActive && x.ProductCategoryId == id).ToList();
            }
            var cate = dbConect.ProductCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Tiltle;
            }
                
            return View(items);
        }
        public ActionResult Details(int id)
        {
            Product p = dbConect.Products.SingleOrDefault(n => n.Id == id);
            TempData["id"]=id;
            ViewBag.DanhMuc = dbConect.ProductCategories.SingleOrDefault(n => n.Id == p.ProductCategoryId).Tiltle;
            return View(p);
        }
        public ActionResult Partial_ItemsByCateId(int id)
        {
            ViewBag.ProductCategoryAlias = dbConect.ProductCategories.Find(id).Alias;
           
            var items = dbConect.Products.Where(x => x.IsHome && x.IsActive && x.ProductCategoryId == id).Take(12).ToList();
            return PartialView(items);
        }
        public ActionResult Partial_ProductSales(int id)
        {
            ViewBag.ProductCategoryAlias = dbConect.ProductCategories.Find(id).Alias;
            //ViewBag.ProductCategoryAlias = new SelectList(dbConect.ProductCategories.ToList(), "Id", "Alias");
            //ViewBag.ProductCategoryAlias = dbConect.ProductCategories.Id;
            //var ProductCategory = dbConect.ProductCategories.Find(id);
            var items = dbConect.Products.Where(x => x.IsHome && x.IsActive && x.ProductCategoryId == id).Take(12).ToList();
            return PartialView(items);
        }
    }
}