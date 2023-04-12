﻿using BanHang.Models.EF;
using BanHang.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Areas.Admin.ViewModels
{
    public class ProductIndexViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ProductDefaultImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ProductCategoryTiltle { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}