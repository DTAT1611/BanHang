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
        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        //var itemOrder = dbConect.Orders.FirstOrDefault(x => x.Code == orderCode);
                        //if (itemOrder != null)
                        //{

                        //    dbConect.Orders.Attach(itemOrder);
                        //    dbConect.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                        //    dbConect.SaveChanges();
                        //}
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
            }

            return View();
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
        public ActionResult CheckOut(OrderViewModel o,string url)
        {


            var code = new { Success = false, Code = -1, Url = "" };
            if (ModelState.IsValid)
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
                        if (p.Quantity <= 0) {
                            p.IsHome = false;
                        }
                    
                    
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
                
                //var url = "";
                if (o.TypePayment == 2)
                {
                    url = UrlPayment(o.TypePaymentVN, order.Code);
                    return Redirect(url);

                }
                


            }

            return RedirectToAction("CheckOutSuccess");
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
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = dbConect.Orders.FirstOrDefault(x => x.Code == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalAmount * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
    }
}