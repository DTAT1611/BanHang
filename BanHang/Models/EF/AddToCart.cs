using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Models.EF
{
    public class AddToCart

    {
        public List<AddToCart> item { get; set; }
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        public int iId { get; set; }
        public string stitle { get; set; }
        
        public int isoluong { get; set; }
        public decimal dprice { get; set; }
        
        public decimal ThanhTien
        {
            get { return dprice * isoluong; }
        }
        public AddToCart(int id, FormCollection f)
        {
            iId = id;
            Product p = dbConect.Products.Single(n => n.Id == iId);
            stitle = p.Title;
            isoluong = int.Parse(f["txtsoluong"].ToString());
            dprice = decimal.Parse(p.Price.ToString());
           
            
                        

        }
        public AddToCart()
        {
            item = new List<AddToCart>();
        }
        
    }
}