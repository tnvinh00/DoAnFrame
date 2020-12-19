using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sky.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int CategoryId { set; get; }
        [Required(ErrorMessage = "Vui lòng nhập đầy đủ")]
        [DisplayName("CategoryName")]
        public string CategoryName { get; set; }



        //1 Danh mục có nhiều sản phẩm
        public List<Product> Products { set; get; }

    }
}
