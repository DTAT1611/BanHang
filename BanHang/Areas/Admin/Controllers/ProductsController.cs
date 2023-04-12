using BanHang.Models.EF;
using BanHang.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanHang.Areas.Admin.ViewModels;

namespace BanHang.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        // GET: Admin/Products
        public ActionResult Index(int? page)
        {
            IEnumerable<Product> Products = dbConect.Products.OrderByDescending(x => x.Id);
            var PreferItemsList = new List<ProductIndexViewModel>();
            var temp1 = dbConect.ProductImages.ToList();
            var temp2 = dbConect.ProductCategories.ToList();
            foreach (Product product in Products)
            {        
                var item = new ProductIndexViewModel()
                {
                    Id = product.Id,
                    Title = product.Title,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    CreatedDate = product.CreatedDate,
                    ProductDefaultImage = temp1.FirstOrDefault(x=>x.ProductId == product.Id && x.IsDefault).Image,
                    ProductCategoryTiltle = temp2.FirstOrDefault(x=>x.Id == product.ProductCategoryId).Tiltle,
                    IsActive= product.IsActive,
                };
                PreferItemsList.Add(item);
            }
            IEnumerable<ProductIndexViewModel> items = PreferItemsList;
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);     
        }
        public ActionResult Add()
        {
            ViewBag.ProductCategory = new SelectList(dbConect.ProductCategories.ToList(), "Id", "Tiltle");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product model, List<string> Images, List<int> rDefault)
        {
            if (ModelState.IsValid)
            {
                if (Images != null && Images.Count > 0)
                {
                    for (int i = 0; i < Images.Count; i++)
                    {
                        if (i + 1 == rDefault[0])
                        {
                            model.ProductImage.Add(new ProductImage
                            {
                                ProductId = model.Id,
                                Image = Images[i],
                                IsDefault = true
                            });
                        }
                        else
                        {
                            model.ProductImage.Add(new ProductImage
                            {
                                ProductId = model.Id,
                                Image = Images[i],
                                IsDefault = false
                            });
                        }
                    }
                }
                model.CreatedDate = DateTime.Now;
                model.ModifierDate = DateTime.Now;
                if (string.IsNullOrEmpty(model.SeoTitle))
                {
                    model.SeoTitle = model.SeoTitle;
                }
                if (string.IsNullOrEmpty(model.Alias))
                    model.Alias = BanHang.Models.Common.Filter.FilterChar(model.Title);
                dbConect.Products.Add(model);
                dbConect.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductCategory = new SelectList(dbConect.ProductCategories.ToList(), "Id", "Tiltle");
            return View(model);
        }
    }
}