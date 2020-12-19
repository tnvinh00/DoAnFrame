using Sky.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sky.Models
{
    [Table("Favorite")]
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }


        //Khoa ngoai
        [Required]
        public string UserId { set; get; }




        public List<FavoriteDetail> FavoriteDetails { set; get; }
    }
}
