using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sky.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int ProductId { set; get; }

        [Required(ErrorMessage = "Vui lòng nhập đầy đủ")]
        [DisplayName("ProductName")]
        public string ProductName { set; get; }

        [DisplayName("ProductImage")]
        public string ProductImage { set; get; }

        [NotMapped]
        [DisplayName("UpLoad File")]
        public IFormFile ProductImageFile { set; get; }

        [Required(ErrorMessage = "Vui lòng nhập đầy đủ")]
        [DisplayName("ProductDescription")]
        public string ProductDescription { set; get; }

        [Required(ErrorMessage = "Vui lòng nhập đầy đủ")]
        [DisplayName("ProductPrice")]
        [Range(0,10000000)]
        public int ProductPrice { set; get; }

        [DefaultValue(0)]
        [DisplayName("PreviousPrice")]
        public int PreviousPrice { set; get; }

        [DisplayName("ViewCount")]
        public int ViewCount { get; set; }

        [DisplayName("ProductDate")]
        public DateTime ProductDate { get; set; }

        [Required]
        public bool ProductStatus { set; get; }

        [Required]
        public int CategoryId { set; get; }
        [Required]
        public int TypeId { set; get; }



        //Khóa ngoại: 1 Sản phẩm thuộc 1 Type và 1 Danh mục
        public Type Type { set; get; }
        public Category Category { set; get; }


        ////1 Đơn hàng có nhiều chi tiết, Bảng chi tiết là bảng trung gian của: Quan hệ n-n DonHang và SanPham
        public List<OrderDetail> OrderDetails { set; get; }
        public List<CartDetail> CartDetails { set; get; }
        public List<FavoriteDetail> FavoriteDetails { set; get; }
    }
}
