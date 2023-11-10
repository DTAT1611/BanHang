using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BanHang.Models.EF
{
    public class Ship : CommonAbstract
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StatusShip { get; set; }
        public ICollection<Order> Orders { get; set; }
        public virtual ApplicationUser ApplicationUsers { get; set; } = null;
    }
}