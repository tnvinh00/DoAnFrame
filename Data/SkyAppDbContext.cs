using Microsoft.EntityFrameworkCore;
using Sky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Type = Sky.Models.Type;

namespace Sky.Data
{
    public class SkyAppDbContext : DbContext
    {
        public SkyAppDbContext(DbContextOptions<SkyAppDbContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            //mỗi giỏ hàng có nhiêu chi tiết GH, mỗi Sản phẩm có nhiều chi tiết GH
            builder.Entity<CartDetail>()
                .HasKey(pk => new { pk.CartId, pk.ProductId });
            builder.Entity<CartDetail>()
                .HasOne(c => c.Cart)
                .WithMany(cd => cd.CartDetails)
                .HasForeignKey(pk => pk.CartId);
            builder.Entity<CartDetail>()
                .HasOne(c => c.Product)
                .WithMany(cd => cd.CartDetails)
                .HasForeignKey(fk => fk.ProductId);

            //mỗi yêu thích có nhiêu chi tiết yêu thích, mỗi Sản phẩm có nhiều chi tiết yêu thích
            builder.Entity<FavoriteDetail>()
                .HasKey(pk => new { pk.FavoriteId, pk.ProductId });
            builder.Entity<FavoriteDetail>()
                .HasOne(f => f.Favorite)
                .WithMany(fd => fd.FavoriteDetails)
                .HasForeignKey(pk => pk.FavoriteId);
            builder.Entity<FavoriteDetail>()
                .HasOne(f => f.Product)
                .WithMany(fd => fd.FavoriteDetails)
                .HasForeignKey(fk => fk.ProductId);

            //mỗi đơn hàng có nhiêu chi tiết DH, mỗi Sản phẩm có nhiều chi tiết DH
            builder.Entity<OrderDetail>()
                .HasKey(pk => new { pk.OrderId, pk.ProductId });
            builder.Entity<OrderDetail>()
                .HasOne(o => o.Order)
                .WithMany(od => od.OrderDetails)
                .HasForeignKey(pk => pk.OrderId);
            builder.Entity<OrderDetail>()
                .HasOne(o => o.Product)
                .WithMany(od => od.OrderDetails)
                .HasForeignKey(fk => fk.ProductId);


            ////////////////////////////////////////////////////////////////////
            //Mỗi user có 1 nhiều đơn hàng
            //builder.Entity<Order>()
            //    .HasOne(o => o.SkyUser)
            //    .WithMany(u => u.Orders)
            //    .HasForeignKey(fk => fk.UserId);

            ////Mỗi user có 1 giỏ hàng
            //builder.Entity<SkyUser>()
            //    .HasOne(c => c.Cart)
            //    .WithOne(u => u.SkyUser)
            //    .HasForeignKey<Cart>(fk => fk.UserId);

            ////Mỗi user có 1 ds yêu thích
            //builder.Entity<SkyUser>()
            //    .HasOne(c => c.Favorite)
            //    .WithOne(u => u.SkyUser)
            //    .HasForeignKey<Favorite>(fk => fk.UserId);


            ////////////////////////////////////////////////////////////////////
            //Mỗi sản phẩm có 1 danh mục
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(fk => fk.CategoryId);

            //Mỗi sản phẩm có 1 loại
            builder.Entity<Product>()
                .HasOne(p => p.Type)
                .WithMany(c => c.Products)
                .HasForeignKey(fk => fk.TypeId);

            base.OnModelCreating(builder);
        }



        public DbSet<Product> ProductDbSet { get; set; }
        public DbSet<Type> TypeDbSet { get; set; }
        public DbSet<Category> CategorieDbSet { get; set; }
        public DbSet<Cart> CartDbSet { get; set; }
        public DbSet<CartDetail> CartDetailDbSet { get; set; }
        public DbSet<Order> OrderDbSet { get; set; }
        public DbSet<OrderDetail> OrderDetailDbSet { get; set; }
        public DbSet<Favorite> FavoriteDbSet { get; set; }
        public DbSet<FavoriteDetail> FavoriteDetailDbSet { get; set; }
    }
}
