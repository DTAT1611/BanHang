using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Models.EF
{
    public class AddToCart

    {
        private ApplicationDbContext dbConect = new ApplicationDbContext();
        public int iId { get; set; }
        public string stitle { get; set; }
        
        public int isoluong { get; set; }
        public double dprice { get; set; }
        
        public double ThanhTien
        {
            get { return dprice * isoluong; }
        }
        public AddToCart(int id, FormCollection f)
        {
            iId = id;
            Product p = dbConect.Products.Single(n => n.Id == iId);
            stitle = p.Title;
            isoluong = int.Parse(f["txtsoluong"].ToString());
            dprice = double.Parse(p.Price.ToString());
           
            
                        

        }
    }
}