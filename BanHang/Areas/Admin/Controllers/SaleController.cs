using BanHang.Models.EF;
using BanHang.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Wordprocessing;

namespace BanHang.Areas.Admin.Controllers
{
    public class SaleController : Controller
    {
        public async Task<int> SaleAPIGetAndApply(int POSTProductID, int Amount)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://127.0.0.1:5000/");

                    var UserInput = new[] { new[] { 1002, 4 } };

                    HttpResponseMessage response = await client.PostAsJsonAsync("GetDiscountVouchers", UserInput);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsAsync<JObject>();

                        if (responseData != null && responseData.TryGetValue("ProductID", out JToken ProductIDToken))
                        {
                            if (int.TryParse(ProductIDToken.ToString(), out int ProductID))
                            {
                                return ProductID;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

            return -1;
        }

        // GET: Admin/Sale
        protected ApplicationDbContext dbConect = new ApplicationDbContext();
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
            var CurrentUserID = User.Identity.GetUserId();
            try
            {
                if (CurrentUserID != null)
                {
                    var FindUserSale = dbConect.Sales.Where(x => x.userid == CurrentUserID)
                                 .OrderByDescending(x => x.CreatedDate)
                                 .FirstOrDefault();
                    if (FindUserSale == null)
                    {
                        var GETProductID = await SaleAPIGetAndApply(0, 0); // Asynchronously wait for the completion of SaleAPIGetAndApply
                        dbConect.Sales.Add(new Sale
                        {
                            userid = User.Identity.GetUserId(),
                            productid = GETProductID,
                            percent = 80,
                            CreatedDate = DateTime.Now,
                            ModifierDate = DateTime.Now,
                            CreatedBy = User.Identity.GetUserId(),
                        });
                        dbConect.SaveChanges();
                        return Json(new { Success = true });
                    }
                    else
                    {
                        var DeltaTime = Convert.ToDateTime(DateTime.Now - dbConect.Sales.Where(x => x.userid == User.Identity.GetUserId()).LastOrDefault().CreatedDate).Day;
                        if (DeltaTime >= 7)
                        {
                            var GETProductID = await SaleAPIGetAndApply(0, 0); // Asynchronously wait for the completion of SaleAPIGetAndApply
                            dbConect.Sales.Add(new Sale
                            {
                                userid = User.Identity.GetUserId(),
                                productid = GETProductID,
                                percent = 85,
                            });
                            dbConect.SaveChanges();
                            return Json(new { Success = true });
                        }
                        else if (DeltaTime >= 4)
                        {
                            var GETProductID = await SaleAPIGetAndApply(0, 0); // Asynchronously wait for the completion of SaleAPIGetAndApply
                            dbConect.Sales.Add(new Sale
                            {
                                userid = User.Identity.GetUserId(),
                                productid = GETProductID,
                                percent = 90,
                                CreatedDate = DateTime.Now,
                            });
                            dbConect.SaveChanges();
                            return Json(new { Success = true });
                        }
                        else
                        {
                            return Json(new { Success = false });
                        }
                    }
                }
                else
                {
                    return Json(new { Success = false });
                }

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { Success = false });
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