using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Sky.Areas.Identity.Pages.Role
{
    [Authorize(Roles = "Manager")]
    public class RoleModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public List<IdentityRole> Roles { set; get; }

        [TempData] // Sử dụng Session lưu thông báo
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Roles = await _roleManager.Roles.ToListAsync();
            return Page();
        }
    }
}
