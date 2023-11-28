using BanHang.Models.EF;
using BanHang.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BanHang.Areas.Admin.Controllers
{
    public class SaleController : Controller
    {
        // GET: Admin/Sale
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        // GET: Admin/Ship
        public ActionResult Index()
        {
            
            return View(dbConect.Sales);
        }
        public PartialViewResult UserSale()
        {
            ViewBag.user = new SelectList(dbConect.Users.Where(x => x.Role == "CUS").ToList(), "Id", "FullName");
            return PartialView();
        }
        [HttpPost]
        public ActionResult UpdateTT(int id,int percent, string userid)
        {
            var item = dbConect.Sales.Find(id);
            if (item != null)
            {
                dbConect.Sales.Attach(item);
                item.userid = userid;
                item.percent = percent;
                dbConect.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }
        public ActionResult AddSale()
        {
            dbConect.Sales.Add(new Sale
            {
                userid = null,
                percent=0,
                CreatedDate = DateTime.Now,
                ModifierDate = DateTime.Now,
                CreatedBy = User.Identity.GetUserId(),
            });
            dbConect.SaveChanges();
            return Json(new { Success = true });
        }

        public async Task<ActionResult> Api()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://127.0.0.1:5000/");
                    //UserInput
                    var UserInput = new[] { new[] { 1002, 4 } };
                    //Make a POST request
                    HttpResponseMessage response = await client.PostAsJsonAsync("GetDiscountVouchers", UserInput);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsAsync<JObject>();
                        if (responseData != null)
                        {
                            var ProductID = responseData["ProductID"].Value<int>();
                            dbConect.Sales.Add(new Sale
                            {
                                userid = User.Identity.GetUserId(),
                                productid = ProductID,
                                percent = 85,
                                CreatedDate = DateTime.Now,
                                ModifierDate = DateTime.Now,
                                CreatedBy = User.Identity.GetUserId(),
                            });
                            dbConect.SaveChanges();
                            return Json(new { Success = true });
                        }
                        else
                            return Json(new { Success = false });
                    }
                    else
                    {
                        return Json(new { Success = false });
                    }
                }
                catch (Exception)
                {
                    return Json(new { Success = false });
                }
            }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = dbConect.Sales.Find(id);
            
            if (item != null)
            {
                dbConect.Sales.Remove(item);
                dbConect.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}