﻿using BanHang.Models;
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
        public ActionResult Partial_ItemsByCateId()
        {
            //ViewBag.ProductCategoryAlias = dbConect.ProductCategories.Find(id).Alias;
            ViewBag.ProductCategoryAlias = new SelectList(dbConect.ProductCategories.ToList(), "Id", "Alias");
            var items = dbConect.Products.Where(x => x.IsHome && x.IsActive).Take(12).ToList();
            return PartialView(items);
        }
    }
}