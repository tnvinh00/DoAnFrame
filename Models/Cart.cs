using Sky.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sky.Models
{
    [Table("Cart")]
    public class Cart
    {
        [Key]
        public int CartId { get; set; }




        [Required]
        public string UserId { get; set; }



        public List<CartDetail> CartDetails { set; get; }

    }
}
