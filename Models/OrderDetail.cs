using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sky.Models
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        public int OrderId { set; get; }
        public int ProductId { set; get; }



        [Required]
        [DisplayName("ProductOrderName")]
        public string ProductOrderName { set; get; }
        [Required]
        [DisplayName("ProductOrderPrice")]
        public string ProductOrderPrice { set; get; }



        [Required]
        [Range(0,100)]
        [DisplayName("Quantity")]
        public int Quantity { set; get; }



        //2 Khóa chính là khóa ngoại
        public Product Product { set; get; }
        public Order Order { set; get; }


        
    }
}
