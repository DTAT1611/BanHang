using BanHang.Models;
using BanHang.Models.EF;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            if(LGH.Count==0)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(LGH);
        }
        public ActionResult CheckOut()
        {

            List<AddToCart> LGH = TakeAddToCart();
            if (LGH.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();

        }
        public ActionResult CheckOutSuccess()
        {

            
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OrderViewModel o)
        {

             
            var code = new { Success = false, Code = -1 };
            if(ModelState.IsValid)
            {
                decimal sum = 0;

                List<AddToCart> LGH = TakeAddToCart();
                Order order = new Order();
                order.CustomerName = o.CustomerName;
                order.Address = o.Address;
                order.Phone = o.Phone;
                order.Email = o.Email;
                foreach (var i in LGH) {
                    if (i != null) { 
                    Product p = dbConect.Products.Find(i.iId);
                    int s = 0;
                    s = p.Quantity - i.isoluong;
                    p.Quantity= s ;
                    
                    
                    order.OrderDetails.Add(new OrderDetail {
                        ProductId = i.iId,
                        Quantity = i.isoluong,
                        Price = i.dprice
                          
                    }) ;
                    
                    sum = sum + (i.dprice * i.isoluong);
                    }
                }
                
                order.TotalAmount = sum;
                order.TypePayment = o.TypePayment;
                order.CreatedDate = DateTime.Now;
                order.ModifierDate = DateTime.Now;
                order.CreatedBy = o.Phone;
                Random rd = new Random();
                order.Code = "Đơn Hàng" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);//todo:add more random
                dbConect.Orders.Add(order);
                
                dbConect.SaveChanges();
                var strSanPham = "";
                var tongtien = decimal.Zero;
                foreach (var item in LGH)
                {
                    strSanPham += "<tr>";
                    strSanPham += "<td>" + item.stitle + "</td>";
                    strSanPham += "<td>" + item.isoluong + "</td>";
                    strSanPham += "<td>" + BanHang.Common.Common.FormatNumber(item.ThanhTien, 0) + "<td>";
                    strSanPham += "</tr>";
                    tongtien += item.dprice * item.isoluong;

                }

                string contentcus = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/send2.html"));
                contentcus = contentcus.Replace("{{MaDon}}", order.Code);
                contentcus = contentcus.Replace("{{SanPham}}", strSanPham);
                contentcus = contentcus.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                contentcus = contentcus.Replace("{{TenKhachHang}}", order.CustomerName);
                contentcus = contentcus.Replace("{{Phone}}", order.Phone);
                contentcus = contentcus.Replace("{{Email}}", o.Email);
                contentcus = contentcus.Replace("{{DiaChiNhanHang}}", order.Address);
                contentcus = contentcus.Replace("{{TongTien}}", BanHang.Common.Common.FormatNumber(tongtien, 0));
                BanHang.Common.Common.SendMail("ShopOnline", "Đơn hàng #" + order.Code, contentcus.ToString(), o.Email);
                string contentad = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/send1.html"));
                contentad = contentad.Replace("{{MaDon}}", order.Code);
                contentad = contentad.Replace("{{SanPham}}", strSanPham);
                contentad = contentad.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                contentad = contentad.Replace("{{TenKhachHang}}", order.CustomerName);
                contentad = contentad.Replace("{{Phone}}", order.Phone);
                contentad = contentad.Replace("{{Email}}", o.Email);
                contentad = contentad.Replace("{{DiaChiNhanHang}}", order.Address);
                contentad = contentad.Replace("{{TongTien}}", BanHang.Common.Common.FormatNumber(tongtien, 0));
                BanHang.Common.Common.SendMail("ShopOnline", "Đơn hàng mới #" + order.Code, contentad.ToString(), ConfigurationManager.AppSettings["Email"]);
                LGH.Clear();
                return RedirectToAction("CheckOutSuccess");
                
            }


            return Json(code);

        }
        public ActionResult CheckOutPartial()
        {
            return PartialView();
        }
        public ActionResult CartPartial()
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
        private decimal TongTien()
        {
            decimal dTongTien=0;
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
                if (sl > 0 && sl <= p.Quantity-gh.isoluong)
                {
                    
                    gh.isoluong = gh.isoluong + sl;
                }

                
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
                return RedirectToAction("Index", "Home");

            }
            if (LGH.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("AddToCart");
        }
    }
}