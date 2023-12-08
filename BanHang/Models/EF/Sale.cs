using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BanHang.Models.EF
{
    public class Sale : CommonAbstract
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string codesale { get; set; }
        public int productid { get; set; }
        public int percent { get; set; }
        public int status { get; set; }
        public string userid { get; set; }
    }
}