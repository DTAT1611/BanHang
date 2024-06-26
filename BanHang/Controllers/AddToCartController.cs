﻿using BanHang.Models;
using BanHang.Models.EF;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.BuilderProperties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Threading;

namespace BanHang.Controllers
{
    public class AddToCartController : Controller
    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        public async Task<int> SaleAPIGetAndApply(int POSTProductID, int Amount)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://127.0.0.1:5000/");

                    var UserInput = new[] { new[] { POSTProductID, Amount } };

                    var CancellationTokenSource = new CancellationTokenSource();
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5), CancellationTokenSource.Token);

                    var apiTask = client.PostAsJsonAsync("GetDiscountVouchers", UserInput);

                    var completedTask = await Task.WhenAny(apiTask, timeoutTask);

                    if (completedTask == apiTask)
                    {
                        HttpResponseMessage response = await apiTask;
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
                    else
                    {
                        CancellationTokenSource.Cancel();
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {

                return -1; 
            }

            return -1;
        }
        public async Task<int> DiscountApi(int ProductID, int Amount)
        {
            var CurrentUserID = User.Identity.GetUserId();
            try
            {
                if (CurrentUserID != null)
                {
                    var FindUserSale = dbConect.Sales.Where(x => x.userid == CurrentUserID)
                                 .OrderByDescending(x => x.CreatedDate)
                                 .FirstOrDefault();
                    var GETProductID = await SaleAPIGetAndApply(ProductID, Amount);
                    if (GETProductID == -1)
                    {
                        return -1;
                    }
                    else
                    {
                        if (FindUserSale == null)
                        {

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
                            return 1;
                        }
                        else
                        {
                            var DeltaTime = Convert.ToDateTime(DateTime.Now - dbConect.Sales.Where(x => x.userid == User.Identity.GetUserId()).LastOrDefault().CreatedDate).Day;
                            if (DeltaTime >= 7)
                            {
                                dbConect.Sales.Add(new Sale
                                {
                                    userid = User.Identity.GetUserId(),
                                    productid = GETProductID,
                                    percent = 85,
                                });
                                dbConect.SaveChanges();
                                return 1;
                            }
                            else if (DeltaTime >= 4)
                            {
                                dbConect.Sales.Add(new Sale
                                {
                                    userid = User.Identity.GetUserId(),
                                    productid = GETProductID,
                                    percent = 90,
                                    CreatedDate = DateTime.Now,
                                });
                                dbConect.SaveChanges();
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }
                }
                else
                {
                    return -1;
                }

            }
            catch (Exception ex)
            {
                return -1;
            }
        }
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
            
            List<AddToCart> LGH = TakeAddToCart();
            
            if (LGH.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            
           
            return View(LGH);
        }

        public PartialViewResult BtnSale()
        {
            var user = User.Identity.GetUserId();
            var item = dbConect.Sales.FirstOrDefault(x=>x.userid==user);
            
            dbConect.SaveChanges();
            return PartialView(item);
        }
        public PartialViewResult CTSales()
        {
            
            List<Sale> hh = new List<Sale>();
            List<AddToCart> LGH = TakeAddToCart();
            
            foreach(var i in LGH)
            {
                var find = dbConect.Sales.Where(x => x.userid == i.userid && x.productid == i.iId).ToList();
                foreach(var item in find)
                {
                    hh.Add(item);
                }
            }
            return PartialView(hh);

        }
        public PartialViewResult IdP()
        {
            int id = Convert.ToInt32(TempData["id"]);
            List<AddToCart> LGH = TakeAddToCart();
            var gh = LGH.Find(x => x.iId == id);
            return PartialView(gh);
        }

        public async Task<ActionResult> VnpayReturn()

        {
            List<AddToCart> LGH = TakeAddToCart();
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
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
                        int sl = 0;
                        Order order = new Order();
                        foreach (var i in LGH)
                        {
                            if (i != null)
                            {
                                Product p = dbConect.Products.Find(i.iId);
                                int s = p.Quantity - i.isoluong;
                                p.Quantity = s;
                                if (p.Quantity <= 0)
                                {
                                    p.IsHome = false;
                                }
                                var find = dbConect.Sales.Where(h=>h.productid==i.iId&&h.status==1).ToList();
                                foreach(var item in find)
                                {
                                    item.status = 2;
                                }

                                order.OrderDetails.Add(new OrderDetail
                                {
                                    ProductId = i.iId,
                                    Quantity = i.isoluong,
                                    Price = i.dprice

                                });
                                sl = sl + i.isoluong;

                            }
                        }
                        order.CustomerName = Convert.ToString(TempData["CustomerName"]);
                        order.Email = Convert.ToString(TempData["Email"]);
                        order.Address = Convert.ToString(TempData["Address"]);
                        order.Phone = Convert.ToString(TempData["Phone"]);
                        order.TotalAmount = Convert.ToDecimal(TempData["TotalAmount"]);
                        order.Code = orderCode;
                        order.TypePayment = 2;
                        order.Status = 2;
                        order.CreatedDate = DateTime.Now;
                        order.ModifierDate = DateTime.Now;
                        order.CreatedBy = order.CustomerName;
                        order.ApplicationUsers = dbConect.Users.Find(User.Identity.GetUserId());
                        order.Quantity = sl;
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
                        dbConect.Orders.Add(order);

                        dbConect.SaveChanges();
                        string contentcus = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/send2.html"));
                        contentcus = contentcus.Replace("{{MaDon}}", orderCode);
                        contentcus = contentcus.Replace("{{SanPham}}", strSanPham);
                        contentcus = contentcus.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                        contentcus = contentcus.Replace("{{TenKhachHang}}", order.CustomerName);
                        contentcus = contentcus.Replace("{{Phone}}", order.Phone);
                        contentcus = contentcus.Replace("{{Email}}", order.Email);
                        contentcus = contentcus.Replace("{{DiaChiNhanHang}}", order.Address);
                        contentcus = contentcus.Replace("{{TongTien}}", BanHang.Common.Common.FormatNumber(tongtien, 0));
                        BanHang.Common.Common.SendMail("ShopOnline", "Đơn hàng #" + order.Code, contentcus.ToString(), order.Email);
                        string contentad = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/send1.html"));
                        contentad = contentad.Replace("{{MaDon}}", order.Code);
                        contentad = contentad.Replace("{{SanPham}}", strSanPham);
                        contentad = contentad.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                        contentad = contentad.Replace("{{TenKhachHang}}", order.CustomerName);
                        contentad = contentad.Replace("{{Phone}}", order.Phone);
                        contentad = contentad.Replace("{{Email}}", order.Email);
                        contentad = contentad.Replace("{{DiaChiNhanHang}}", order.Address);
                        contentad = contentad.Replace("{{TongTien}}", BanHang.Common.Common.FormatNumber(tongtien, 0));
                        BanHang.Common.Common.SendMail("ShopOnline", "Đơn hàng mới #" + order.Code, contentad.ToString(), ConfigurationManager.AppSettings["Email"]);
                        LGH.Clear();
                        //Starting Discount Api
                        var FindOrderID = dbConect.Orders.Where(x => x.Code == order.Code).FirstOrDefault().Id;
                        var ListOrderDetails = dbConect.OrderDetails.Where(x => x.OrderId == FindOrderID).ToList();
                        OrderDetail ItemForDiscount = null;
                        int MaxAmount = 4;
                        foreach(var item in ListOrderDetails)
                        {
                            if (ItemForDiscount != null && item.Quantity > MaxAmount)
                            {
                                MaxAmount = item.Quantity;
                                ItemForDiscount = item;
                            }
                            else
                            {
                                MaxAmount = item.Quantity;
                                ItemForDiscount = item;
                            }
                        }
                        var result = await DiscountApi(ItemForDiscount.ProductId, MaxAmount);
                        //End Discount Api
                    }
                    else
                    {
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        return View();

                    }

                    ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                    ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                    
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
        public async Task<ActionResult> CheckOut(OrderViewModel o)
        {
            
            if (ModelState.IsValid)
            {
                decimal sum = 0;
                int sl = 0;
                List<AddToCart> LGH = TakeAddToCart();
                Order order = new Order();
                Random rd = new Random();
                foreach (var i in LGH)
                {
                    if (i != null)
                    {
                        Product p = dbConect.Products.Find(i.iId);
                        int s = 0;
                        s = p.Quantity - i.isoluong;
                        p.Quantity = s;
                        if (p.Quantity <= 0)
                        {
                            p.IsHome = false;
                        }
                        var find = dbConect.Sales.Where(h => h.productid == i.iId && h.status == 1).ToList();
                        foreach (var item in find)
                        {
                            item.status = 2;
                        }

                        order.OrderDetails.Add(new OrderDetail
                        {
                            ProductId = i.iId,
                            Quantity = i.isoluong,
                            Price = i.dprice

                        });
                        sl = sl+i.isoluong;
                        sum = sum + (i.dprice * i.isoluong);
                    }
                }
                var userid = User.Identity.GetUserId();
                order.CustomerName = dbConect.Users.FirstOrDefault(x => x.Id == userid).FullName;
                order.Address = o.Address;
                order.Phone = dbConect.Users.FirstOrDefault(x => x.Id == userid).Phone;
                order.Email = dbConect.Users.FirstOrDefault(x => x.Id == userid).Email;
                order.TotalAmount = sum;
                if (o.TypePayment == 2)
                {

                    
                    if (o.TypePaymentVN == 0) {
                        TempData["CustomerName"] = order.CustomerName;
                        TempData["Email"] = order.Email;
                        TempData["Address"] = order.Address;
                        TempData["Phone"]= order.Phone;
                        TempData["TotalAmount"] = order.TotalAmount;
                        order.Code = "Đơn Hàng " + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                        order.CreatedDate = DateTime.Now;
                        string url = UrlPayment(o.TypePaymentVN, order.Code, order.TotalAmount, order.CreatedDate);
                        
                        return Redirect(url);
                    }
 
                }
                order.Quantity = sl;
                order.TypePayment = o.TypePayment;
                order.CreatedDate = DateTime.Now;
                order.ModifierDate = DateTime.Now;
                order.Status=1;
                order.CreatedBy = order.CustomerName;
                order.ApplicationUsers = dbConect.Users.Find(User.Identity.GetUserId());
                order.Code = "Đơn Hàng " + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
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
                contentcus = contentcus.Replace("{{Email}}", order.Email);
                contentcus = contentcus.Replace("{{DiaChiNhanHang}}", order.Address);
                contentcus = contentcus.Replace("{{TongTien}}", BanHang.Common.Common.FormatNumber(tongtien, 0));
                BanHang.Common.Common.SendMail("ShopOnline", "Đơn hàng #" + order.Code, contentcus.ToString(), order.Email);
                string contentad = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/send1.html"));
                contentad = contentad.Replace("{{MaDon}}", order.Code);
                contentad = contentad.Replace("{{SanPham}}", strSanPham);
                contentad = contentad.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                contentad = contentad.Replace("{{TenKhachHang}}", order.CustomerName);
                contentad = contentad.Replace("{{Phone}}", order.Phone);
                contentad = contentad.Replace("{{Email}}", order.Email);
                contentad = contentad.Replace("{{DiaChiNhanHang}}", order.Address);
                contentad = contentad.Replace("{{TongTien}}", BanHang.Common.Common.FormatNumber(tongtien, 0));
                BanHang.Common.Common.SendMail("ShopOnline", "Đơn hàng mới #" + order.Code, contentad.ToString(), ConfigurationManager.AppSettings["Email"]);
                LGH.Clear();
                //Starting Discount Api
                var FindOrderID = dbConect.Orders.Where(x=>x.Code == order.Code).FirstOrDefault().Id;
                var ListOrderDetails = dbConect.OrderDetails.Where(x => x.OrderId == FindOrderID).ToList();
                OrderDetail ItemForDiscount = null;
                int MaxAmount = 4;
                foreach (var item in ListOrderDetails)
                {
                    if (ItemForDiscount != null && item.Quantity > MaxAmount)
                    {
                        MaxAmount = item.Quantity;
                        ItemForDiscount = item;
                    }
                    else
                    {
                        MaxAmount = item.Quantity;
                        ItemForDiscount = item;
                    }
                }
                var result = await DiscountApi(ItemForDiscount.ProductId, MaxAmount);
                //End Discount Api
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
                    gh.userid = User.Identity.GetUserId();
                    var find = dbConect.Sales.Where(n => n.productid==gh.iId&&n.status==1).ToList();
                    foreach(var i in find)
                    {
                        i.status = 0;
                    }
                    dbConect.SaveChanges();
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
                var find = dbConect.Sales.Where(s => s.productid==gh.iId&&s.status==1).ToList();
                foreach(var item in find)
                {
                    item.status = 0;
                }
                dbConect.SaveChanges();
                LGH.RemoveAll(n => n.iId == gh.iId);
                return RedirectToAction("EditAddToCart", "AddToCart");

            }
            if (LGH.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("AddToCart");
        }
        public ActionResult UseSale(string ids)
        {
            List<AddToCart> LGH = TakeAddToCart();
           
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = dbConect.Sales.Find(Convert.ToInt32(item));
                        
                        var p = dbConect.Products.Find(obj.productid);

                        AddToCart gh = LGH.SingleOrDefault(n=>n.iId==obj.productid);

                        if (obj.status==0)
                        {
                            decimal newprice = 0;
                            newprice = gh.dprice - ((gh.dprice*obj.percent)/100);
                            gh.dprice = newprice;
                            obj.status = 1;
                        }
                       
                        dbConect.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = true });
        }
        public ActionResult CancleSale(int id)
        {
            TempData["id"] = id;
            List<AddToCart> LGH = TakeAddToCart();
            var sale = dbConect.Sales.Find(id);
            var p= dbConect.Products.Find(sale.productid);
            AddToCart gh = LGH.SingleOrDefault(n => n.iId == sale.productid);
            if(sale.status==1)
            {
                if (p.IsSale)
                {
                    gh.dprice = p.PriceSale;
                }
                else
                {
                    gh.dprice = p.Price;
                }
                sale.status = 0;
            }
            dbConect.SaveChanges();
            return Json(new { success = true });
        }
        public string UrlPayment(int TypePaymentVN, string orderCode,decimal total,DateTime date)
        {
            OrderViewModel o = new OrderViewModel();
            o.Code= orderCode;
            o.TotalAmount= total;
            o.CreatedDate = date;
            var urlPayment = "";
            
            
            
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)o.TotalAmount * 100;
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

            vnpay.AddRequestData("vnp_CreateDate", o.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng: " + o.Code);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", o.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing
            

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            
            
            
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
    }
}