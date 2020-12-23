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
        [DisplayName("Tên sản phẩm")]
        public string ProductName { set; get; }

        [DisplayName("Hình ảnh")]
        public string ProductImage { set; get; }

        [NotMapped]
        [DisplayName("UpLoad File")]
        public IFormFile ProductImageFile { set; get; }

        [Required(ErrorMessage = "Vui lòng nhập đầy đủ")]
        [DisplayName("Mô tả sản phẩm")]
        public string ProductDescription { set; get; }

        [Required(ErrorMessage = "Vui lòng nhập đầy đủ")]
        [DisplayName("Giá sản phẩm")]
        [Range(0,10000000)]
        public int ProductPrice { set; get; }

        [DefaultValue(0)]
        [DisplayName("Giá trước khuyến mãi (Điền 0 nếu muốn để trống)")]
        public int PreviousPrice { set; get; }

        [DisplayName("Lượt xem")]
        public int ViewCount { get; set; }

        [DisplayName("Ngày tạo sản phẩm")]
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
