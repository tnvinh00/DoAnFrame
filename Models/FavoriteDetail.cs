using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sky.Models
{
    [Table("FavoriteDetail")]
    public class FavoriteDetail
    {
        public int FavoriteId { set; get; }
        public int ProductId { set; get; }


        //


        public Favorite Favorite { set; get; }
        public Product Product { set; get; }
    }
}
