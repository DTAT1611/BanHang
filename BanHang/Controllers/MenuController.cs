﻿using BanHang.Models;
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
    }
}