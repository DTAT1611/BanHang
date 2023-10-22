using BanHang.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanHang.Models
{
    public class commentsviewmodel : CommonAbstract
    {
        public ProductImage productImages { get; set; }
        public Product products { get; set; }
        public Comment comments {get; set; }
    }
}