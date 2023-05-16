using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BanHang.Models
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Tên Khách Hàng không để trống")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "số điện thoại không để trống")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Địa chỉ không để trống")]
        public string Address { get; set; }
        public string Email { get; set; }
        public int TypePayment { get; set; }
    }
}