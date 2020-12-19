using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sky.Areas.Identity.Data;
using Sky.Data;

[assembly: HostingStartup(typeof(Sky.Areas.Identity.IdentityHostingStartup))]
namespace Sky.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<SkyDbContext>(options =>
                    options.UseMySql(
                        context.Configuration.GetConnectionString("SkyDbContextConnection")));

                services.AddDefaultIdentity<SkyUser>(options => {
                    //Xác nhận email
                    options.SignIn.RequireConfirmedAccount = true;
                    
                    //Yêu cầu xác nhận Email
                    options.SignIn.RequireConfirmedEmail = true;
                    //Yêu cầu có chữ hoa
                    options.Password.RequireLowercase = false;
                    //Yêu cầu có chữ thường
                    options.Password.RequireUppercase = false;
                    //Yêu cầu có số
                    options.Password.RequireDigit = false;
                    //Yêu cầu có kí tự không phải chữ và số
                    options.Password.RequireNonAlphanumeric = false;
                    //Yêu cầu độ dài
                    options.Password.RequiredLength = 6;
                    //Yêu cầu kí tự đặc biệt
                    options.Password.RequiredUniqueChars = -1;

                    //Thời gian mở khóa trở lại
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
                    //Số lần thử
                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.AllowedForNewUsers = true;
                })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<SkyDbContext>();
            });
        }
    }
}