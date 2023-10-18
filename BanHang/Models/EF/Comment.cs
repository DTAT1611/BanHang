using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Models.EF
{
    [Table("tb_Comments")]
    public class Comment : CommonAbstract
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string comms { get; set; }
        public virtual Product Product { get; set; }
        public virtual ApplicationUser ApplicationUsers { get; set; }
        public int Reply { get; set; }

    }
}