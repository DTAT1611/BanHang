using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BanHang.Models.EF
{
    [Table("tb_Advs")]
    public class Adv : CommonAbstract
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Title { get; set; }
        [StringLength(500, ErrorMessage = "Không được vượt quá 500 ký tự")]
        public string Description { get; set; }
        [StringLength(500)]
        public string Link { get; set; }
        public int Type { get; set; }
    }
}