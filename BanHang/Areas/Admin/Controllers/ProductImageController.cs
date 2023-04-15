using BanHang.Models;
using BanHang.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace BanHang.Areas.Admin.Controllers
{
    public class ProductImageController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        public ActionResult Index(int id)
        {
            ViewBag.ProductId = id;
            var items = dbConect.ProductImages.Where(x=>x.ProductId == id).ToList();
            return View(items);
        }

        [HttpPost]
        public ActionResult AddImage(int productId, string url)
        {
            
            dbConect.ProductImages.Add(new ProductImage
            {
                ProductId = productId,
                Image = url,
                IsDefault = false
            });
            dbConect.SaveChanges();
            return Json(new { Success = true });
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = dbConect.ProductImages.Find(id);
            dbConect.ProductImages.Remove(item);
            dbConect.SaveChanges();
            return Json(new { success = true });
        }
    }
}