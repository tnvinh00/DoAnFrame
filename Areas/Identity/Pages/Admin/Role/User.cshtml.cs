using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Sky.Areas.Identity.Data;

namespace Sky.Areas.Identity.Pages.Role
{
    [Authorize(Roles = "Manager")]
    public class UserModel : PageModel
    {
        const int USER_PER_PAGE = 10;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<SkyUser> _userManager;

        public UserModel(RoleManager<IdentityRole> roleManager,
                          UserManager<SkyUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public class UserInList : SkyUser
        {
            // Liệt kê các Role của User ví dụ: "Admin,Editor" ...
            public string Listroles { set; get; }
        }

        public List<UserInList> users;
        public int TotalPages { set; get; }

        [TempData] // Sử dụng Session
        public string StatusMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { set; get; }

        public string UserNameSort { get; set; }
        public string FullNameSort { get; set; }
        public string RoleSort { get; set; }
        public string CurrentSort { get; set; }

        public IActionResult OnPost() => NotFound("Cấm post");

        public async Task<IActionResult> OnGet(string sortOrder, string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            CurrentSort = sortOrder;
            UserNameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            FullNameSort = sortOrder == "full" ? "full_desc" : "full";

            if (PageNumber == 0)
                PageNumber = 1;

            var lusers = (from u in _userManager.Users
                          orderby u.UserName
                          select new UserInList()
                          {
                              Id = u.Id,
                              UserName = u.UserName,
                              FullName = u.FullName,
                          });

            if (!String.IsNullOrEmpty(searchString))
            {
                lusers = lusers.Where(u => u.UserName.Contains(searchString) || u.FullName.Contains(searchString));
            }

            //Sort
            switch (sortOrder)
            {
                case "name_desc":
                    lusers = lusers.OrderByDescending(s => s.UserName);
                    break;
                case "full":
                    lusers = lusers.OrderBy(s => s.FullName);
                    break;
                case "full_desc":
                    lusers = lusers.OrderByDescending(s => s.FullName);
                    break;
                default:
                    lusers = lusers.OrderBy(s => s.UserName);
                    break;
            }

            if (!lusers.Any())
            {
                StatusMessage = "Không tìm thấy từ khóa '" + searchString + "'";
            }
            else
            {
                StatusMessage = "";
            }
            
            int totalUsers = await lusers.CountAsync();


            TotalPages = (int)Math.Ceiling((double)totalUsers / USER_PER_PAGE);

            users = await lusers.Skip(USER_PER_PAGE * (PageNumber - 1)).Take(USER_PER_PAGE).ToListAsync();

            // users.ForEach(async (user) => {
            //     var roles = await _userManager.GetRolesAsync(user);
            //     user.listroles = string.Join(",", roles.ToList());
            // });

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Listroles = string.Join(",", roles.ToList());
            }

            return Page();
        }
    }
}
