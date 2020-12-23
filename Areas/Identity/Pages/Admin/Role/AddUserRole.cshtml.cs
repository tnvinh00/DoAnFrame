using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Sky.Areas.Identity.Data;
using static Sky.Data.Helper;

namespace Sky.Areas.Identity.Pages.Role
{
    [Authorize(Roles = "Manager")]

    public class AddUserRoleModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<SkyUser> _userManager;


        public AddUserRoleModel(RoleManager<IdentityRole> roleManager,
                            UserManager<SkyUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public class InputModel
        {
            [Required]
            public string ID { set; get; }
            public string Name { set; get; }

            public string[] RoleNames { set; get; }

        }

        [BindProperty]
        public InputModel Input { set; get; }

        [BindProperty]
        public bool IsConfirmed { set; get; }

        [TempData] // Sử dụng Session
        public string StatusMessage { get; set; }

        public IActionResult OnGet() => NotFound("Không thấy");

        public List<string> AllRoles { set; get; } = new List<string>();

        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.FindByIdAsync(Input.ID);
            if (user == null)
            {
                return NotFound("Không thấy role cần xóa");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var allroles = await _roleManager.Roles.ToListAsync();

            allroles.ForEach((r) => {
                AllRoles.Add(r.Name);
            });

            if (!IsConfirmed)
            {
                Input.RoleNames = roles.ToArray();
                IsConfirmed = true;
                StatusMessage = "";
                ModelState.Clear();
            }
            else
            {
                // Update add and remove
                StatusMessage = "Vừa cập nhật";
                if (Input.RoleNames == null) Input.RoleNames = new string[] { };
                foreach (var rolename in Input.RoleNames)
                {
                    if (roles.Contains(rolename)) continue;
                    await _userManager.AddToRoleAsync(user, rolename);
                }
                foreach (var rolename in roles)
                {
                    if (Input.RoleNames.Contains(rolename)) continue;
                    await _userManager.RemoveFromRoleAsync(user, rolename);
                }

            }

            Input.Name = user.UserName;
            return Page();
        }
    }
}
