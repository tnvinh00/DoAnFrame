using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sky.Models
{
    [Table("Type")]
    public class Type
    {
        [Key]
        public int TypeId { set; get; }

        [Required(ErrorMessage = "Vui lòng nhập đầy đủ")]
        [DisplayName("TypeName")]
        public string TypeName { get; set; }



        //1 loại có nhiều sản phẩm
        public List<Product> Products { set; get; }
    }
}
