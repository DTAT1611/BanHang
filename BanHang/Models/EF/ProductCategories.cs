using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BanHang.Models.EF
{
    [Table("tb_ProductCategories")]
    public class ProductCategories : CommonAbstract
    {
        public ProductCategories()
        {
            this.Products = new HashSet<Product>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Tiltle { get; set; }
        [Required]
        [StringLength(250)]
        public string Alias { get; set; }
        [StringLength(250, ErrorMessage = "Không được vượt quá 250 ký tự")]
        public string Discription { get; set; }
        
        public string Icon { get; set; }
        
        public string SeoTitle { get; set; }
        [StringLength(500)]
        public string SeoDiscription { get; set; }
        [StringLength(250)]
        public string SeoKeywords { get; set; }
        public ICollection<Product> Products { get; set; }
    }
    }