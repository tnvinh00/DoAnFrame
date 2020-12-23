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

namespace Sky.Areas.Identity.Pages.Admin.User
{
    [Authorize(Roles = "Admin, Manager")]
    public class LockUserModel : PageModel
    {
        const int USER_PER_PAGE = 15;
        private readonly UserManager<SkyUser> _userManager;

        public LockUserModel(UserManager<SkyUser> userManager)
        {
            _userManager = userManager;
        }
        public int TotalPages { set; get; }

        [TempData] // Sử dụng Session
        public string StatusMessage { get; set; }

        [TempData] // Sử dụng Session
        public string Notification { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { set; get; }

        public class UserInList : SkyUser
        {
            // Liệt kê các Role của User ví dụ: "Admin,Editor" ...
            public string Listroles { set; get; }
        }

        public List<UserInList> users;

        public string UserNameSort { get; set; }
        public string FullNameSort { get; set; }
        public string CurrentSort { get; set; }
        public async Task<IActionResult> OnGetAsync(string sortOrder, string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            CurrentSort = sortOrder;
            UserNameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            FullNameSort = sortOrder == "full" ? "full_desc" : "full";

            if (PageNumber == 0)
                PageNumber = 1;

            var lusers = (from u in _userManager.Users
                          where u.LockoutEnabled == false
                          select new UserInList()
                          {
                              Id = u.Id,
                              UserName = u.UserName,
                              FullName = u.FullName,
                              CMND = u.CMND,
                              EmailConfirmed = u.EmailConfirmed,
                              PhoneNumber = u.PhoneNumber,
                              LockoutEnabled = u.LockoutEnabled
                          }); ;

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

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Listroles = string.Join(",", roles.ToList());
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            user.LockoutEnabled = true;

            await _userManager.UpdateAsync(user);

            Notification = "Đã mở khóa tài khoản " + user.Email;

            return RedirectToPage("UnLockUser");
        }
    }
}
