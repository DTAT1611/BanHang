using BanHang.Models;
using BanHang.Models.EF;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace BanHang.Controllers
{
    public class AddToCartController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        public List<AddToCart> TakeAddToCart()
        {

            List<AddToCart> LGH = Session["AddToCart"] as List<AddToCart>;
            if (LGH == null)
            {
                LGH = new List<AddToCart>();
                Session["AddToCart"] = LGH;
            }
            return LGH;

        }
        // GET: AddToCart
        public ActionResult AddToCart()
        {
            if (Session["AddToCart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<AddToCart> LGH = TakeAddToCart();
            return View(LGH);
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<AddToCart> LGH = Session["AddToCart"] as List<AddToCart>;
            if (LGH != null)
            {
                iTongSoLuong = LGH.Sum(n => n.isoluong);
            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double dTongTien = 0;
            List<AddToCart> LGH = Session["AddToCart"] as List<AddToCart>;
            if (LGH != null)
            {
                dTongTien = LGH.Sum(n => n.ThanhTien);
            }
            return dTongTien;
        }
        public ActionResult AddAddToCart(int intid, FormCollection f,  string strURL)
        {
            int sl = 0;
            Product p = dbConect.Products.SingleOrDefault(n =>n.Id == intid);

            List<AddToCart> LGH = TakeAddToCart();
            AddToCart gh = LGH.Find(n => n.iId == intid);
            
            if (gh == null)
            {
                gh = new AddToCart(intid, f);
                
                
                if(gh.isoluong > 0 &&gh.isoluong <= p.Quantity)
                {
                    LGH.Add(gh);
                }
                return Redirect(strURL);



            }


            else if(gh!=null)
            {
                sl = int.Parse(f["txtsoluong"].ToString());
                gh.isoluong = gh.isoluong + sl;
                return Redirect(strURL);
            }
            return Redirect(strURL);






        }
        public ActionResult AddToCartPartial()
        {
            if (TongSoLuong() == 0)
            {
                return PartialView();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult EditAddToCart()
        {
            if (Session["AddToCart"] == null)
            {
                return RedirectToAction("Index","Home");
            }
            List<AddToCart> LGH = TakeAddToCart();
            return View(LGH);
        }
        public ActionResult UpdateAddToCart(int ip, FormCollection f, string strURL)
        {
            int sl = 0;
            Product p = dbConect.Products.SingleOrDefault(n => n.Id == ip);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<AddToCart> LGH = TakeAddToCart();
            AddToCart gh = LGH.SingleOrDefault(n => n.iId == ip);
            sl = int.Parse(f["txtsoluong"].ToString());
            
            if(sl>0&&sl <= p.Quantity) 
            {
                
                gh.isoluong = sl;
                return RedirectToAction("AddToCart");
            }
            return RedirectToAction("AddToCart");




        }
        public ActionResult DeleteAddToCart(int ip)
        {
            Product p = dbConect.Products.SingleOrDefault(n => n.Id == ip);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<AddToCart> LGH = TakeAddToCart();
            AddToCart gh = LGH.SingleOrDefault(n => n.iId == ip);
            if (gh != null)
            {
                LGH.RemoveAll(n => n.iId == gh.iId);

            }
            if (LGH.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("AddToCart");
        }
    }
}