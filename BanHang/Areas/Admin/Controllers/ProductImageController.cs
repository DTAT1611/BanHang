using BanHang.Models;
using BanHang.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
            var L_I_Counter = dbConect.ProductImages.Count();
            if(L_I_Counter > 1 )
            {
                if(item.IsDefault)
                {
                    dbConect.ProductImages.First(x => x.Id != id).IsDefault = true; ;
                    dbConect.ProductImages.Remove(item);
                    dbConect.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    dbConect.ProductImages.Remove(item);
                    dbConect.SaveChanges();
                    return Json(new { success = true });
                }
            }
            else
            {
                return Json(new { success = false });
            }
            
        }
        [HttpPost]
        public ActionResult ChangeDefaultState(int Id)
        {
            var PrevDefaultState = dbConect.ProductImages.First(x => x.IsDefault);
            var item = dbConect.ProductImages.Find(Id);
            PrevDefaultState.IsDefault = false;
            item.IsDefault = true;
            dbConect.ProductImages.AddOrUpdate(item);
            dbConect.ProductImages.AddOrUpdate(PrevDefaultState);
            dbConect.SaveChanges();
            return Json(new { Success = true });
        }
    }
}