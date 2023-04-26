using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

namespace BanHang.Models.EF
{
    [Table("tb_Category")]
    public class Category : CommonAbstract
    {
            public Category()
            {
                this.News = new HashSet<New>();
            }
             [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }
            [StringLength(150)]
            [Required(ErrorMessage = "Không để trống")]
            public string Tiltle { get; set; }
            public string Alias { get; set; }
            public string Link { get; set; }
            [StringLength(250, ErrorMessage = "Không được vượt quá 250 ký tự")]
            public string Discription { get; set; }
            public int Position { get; set; }
            [StringLength(150)]
            public string SeoTitle { get; set; }
            [StringLength(150)]
            public string SeoDiscription { get; set; }
            [StringLength(150)]
            public string SeoKeywords { get; set; }
            public bool IsActive { get; set; }

            public ICollection<New> News { get; set; }
            public ICollection<Post> Posts { get; set; }

        }
    }