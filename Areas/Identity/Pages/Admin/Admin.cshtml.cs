using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sky.Areas.Identity.Data;
using Sky.Data;

namespace Sky.Areas.Identity.Pages.Admin
{
    [Authorize(Roles = "Admin, Manager")]
    public class AdminModel : PageModel
    {
        private readonly UserManager<SkyUser> _userManager;
        private readonly SkyDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminModel(UserManager<SkyUser> userManager,
            SkyDbContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public List<int> percent = new List<int>();
        public List<string> lable = new List<string>();

        public void OnPost() => NotFound();
        public async Task OnGetAsync()
        {
            var user = _context.Roles.Where(c => c.Name == "User").FirstOrDefault();
            var admin = _context.Roles.Where(c => c.Name == "Admin").FirstOrDefault();
            var manager = _context.Roles.Where(c => c.Name == "Manager").FirstOrDefault();


            var skyAppDbContext0 = _context.Users.Where(c => c.EmailConfirmed == true)
                .Join(_context.UserRoles.Where(b => b.RoleId == user.Id), c => c.Id, d => d.UserId, (c, d) => new { c, d }).ToList();
            var skyAppDbContext1 = _context.Users.Where(c => c.EmailConfirmed == false).ToList();
            var skyAppDbContext2 = _context.Users
                .Join(_context.UserRoles.Where(b => b.RoleId == admin.Id), c => c.Id, d => d.UserId, (c, d) => new { c, d }).ToList();
            var skyAppDbContext3 = _context.Users
                .Join(_context.UserRoles.Where(b => b.RoleId == manager.Id), c => c.Id, d => d.UserId, (c, d) => new { c, d }).ToList();

            int sum = skyAppDbContext0.Count() + skyAppDbContext1.Count() + skyAppDbContext2.Count() + skyAppDbContext3.Count();

            ViewData["sum"] = sum;
            
            //Phần trăm
            percent.Add(skyAppDbContext0.Count());
            percent.Add(skyAppDbContext1.Count());
            percent.Add(skyAppDbContext2.Count());
            percent.Add(skyAppDbContext3.Count());

            //Số user
            lable.Add("User (" + skyAppDbContext0.Count() + ")");
            lable.Add("User not Confirmed (" + skyAppDbContext1.Count() + ")");
            lable.Add("Admin (" + skyAppDbContext2.Count() + ")");
            lable.Add("Manager (" + skyAppDbContext3.Count() + ")");

            //
            var curr = await _userManager.GetUserAsync(User);

            var roles = await _userManager.GetRolesAsync(curr);
            ViewData["Role"] = string.Join(",", roles.ToList());
        }
    }
}
