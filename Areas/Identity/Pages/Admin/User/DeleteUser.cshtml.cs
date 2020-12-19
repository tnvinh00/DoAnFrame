using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sky.Areas.Identity.Data;

namespace Sky.Areas.Identity.Pages.Admin.User
{
    [Authorize(Roles = "Admin, Manager")]
    public class DeleteUserModel : PageModel
    {
        private readonly UserManager<SkyUser> _userManager;
        public DeleteUserModel(UserManager<SkyUser> userManager)
        {
            _userManager = userManager;
        }

        public class InputModel
        {
            [Required]
            public string ID { set; get; }
            public string UserName { set; get; }

        }

        [BindProperty]
        public InputModel Input { set; get; }

        [BindProperty]
        public bool IsConfirmed { set; get; }

        [TempData] // Sử dụng Session
        public string StatusMessage { get; set; }

        public IActionResult OnGet() => NotFound("Không thấy");

        public async Task<IActionResult> OnPost()
        {

            if (!ModelState.IsValid)
            {
                return NotFound("Không xóa được");
            }

            var user = await _userManager.FindByIdAsync(Input.ID);
            if (user == null)
            {
                return NotFound("Không thấy role cần xóa");
            }

            ModelState.Clear();

            if (IsConfirmed)
            {
                //Xóa
                await _userManager.DeleteAsync(user);
                StatusMessage = "Đã xóa " + user.UserName;

                return RedirectToPage("Index");
            }
            else
            {
                Input.UserName = user.UserName;
                IsConfirmed = true;

            }

            return Page();
        }
    }
}
