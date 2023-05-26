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
        [Required(ErrorMessage = "Tên Khách Hàng không để trống")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "số điện thoại không để trống")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Địa chỉ không để trống")]
        public string Address { get; set; }
        public string Email { get; set; }
        public decimal TotalAmount { get; set; }
        public int TypePayment { get; set; }
        public int TypePaymentVN { get; set; }
    }
}