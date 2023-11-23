using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BanHang.Models
{
    public class OrderViewModel : CommonAbstract
    {
        public string Code { get; set; }
        public string Address { get; set; }
        public decimal TotalAmount { get; set; }
        public int TypePayment { get; set; }
        public int TypePaymentVN { get; set; }
    }
}