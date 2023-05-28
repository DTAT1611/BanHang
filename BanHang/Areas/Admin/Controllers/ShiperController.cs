﻿using BanHang.Models;
using BanHang.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Areas.Admin.Controllers
{
    public class ShiperController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();`
        // GET: Admin/Shiper
        public ActionResult Index()
        {
            var items = dbConect.Shippers;
            return View(items);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Shipper shipper)
        {
            dbConect.Shippers.Add(shipper);
            dbConect.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}