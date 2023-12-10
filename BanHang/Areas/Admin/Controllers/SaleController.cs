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
using DocumentFormat.OpenXml.Spreadsheet;


namespace BanHang.Areas.Admin.Controllers
{
    public class SaleController : Controller
    {
        // GET: Admin/Sale
        protected ApplicationDbContext dbConect = new ApplicationDbContext();
        // GET: Admin/Ship
        public ActionResult Index()
        {
            //ViewBag.suser =dbConect.Sales.FirstOrDefault().userid;
            return View(dbConect.Sales);
        }
        public PartialViewResult UserSale()
        {
            ViewBag.user = new SelectList(dbConect.Users.Where(x => x.Role == "CUS").ToList(), "Id", "FullName");
            return PartialView();
        }
        public PartialViewResult ProductSale()
        {
            ViewBag.product = new SelectList(dbConect.Products.ToList(),"Id","Title");
            return PartialView();
        }
        [HttpPost]
        public ActionResult UpdateTT(int id,int percent, string userid,int productid)
        {
            var item = dbConect.Sales.Find(id);
            
            ViewBag.sproduct = productid;
            if (item != null)
            {
                dbConect.Sales.Attach(item);
                item.userid = userid;
                if (percent >= 30&& percent<=70)
                {
                    item.percent = percent;
                }
                else
                {
                    return Json(new { message = "Unsuccess", Success = false });
                }
                item.codesale = "SALECODE8"+percent+"%";
                if (0 == dbConect.Sales.Where(x=>x.productid==productid&&x.userid== userid&&x.status!=2).Count())
                {
                    item.productid = productid;
                }
                else
                {
                    return Json(new { message = "Unsuccess", Success = false });
                }
                
                item.productid = productid;
                
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
                codesale="SALECODE8",
                productid=0,
                percent=0,
                CreatedDate = DateTime.Now,
                ModifierDate = DateTime.Now,
                CreatedBy = User.Identity.GetUserId(),
            });
            
            dbConect.SaveChanges();
            return Json(new { Success = true });
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