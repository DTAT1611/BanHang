﻿using BanHang.Models;
using BanHang.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Partial_ItemsByCateId(int id)
        {
            ViewBag.ProductCategoryAlias = dbConect.ProductCategories.Find(id).Alias;
            //ViewBag.ProductCategoryAlias = new SelectList(dbConect.ProductCategories.ToList(), "Id", "Alias");
            //ViewBag.ProductCategoryAlias = dbConect.ProductCategories.Id;
            //var ProductCategory = dbConect.ProductCategories.Find(id);
            var items = dbConect.Products.Where(x => x.IsHome && x.IsActive).Take(12).ToList();
            var products = dbConect.Products.Where(p => p.ProductCategories.Id == id).ToList();
            return PartialView(items);
        }
    }
}