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
    public class EditUserModel : PageModel
    {
        private readonly UserManager<SkyUser> _userManager;
        public EditUserModel(UserManager<SkyUser> userManager)
        {
            _userManager = userManager;
        }

        public class InputModel
        {
            [Required]
            public string ID { set; get; }
            public string UserName { set; get; }
            public string FullName { set; get; }
            public string CMND { get; set; }
            public string PhoneNumber { get; set; }
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
                return NotFound("Không sửa được");
            }

            var user = await _userManager.FindByIdAsync(Input.ID);
            if (user == null)
            {
                return NotFound("Không thấy user cần xóa");
            }

            ModelState.Clear();

            if (IsConfirmed)
            {
                //Xóa
                user.PhoneNumber = Input.PhoneNumber;
                user.CMND = Input.CMND;
                user.FullName = Input.FullName;
                var userUpdate = await _userManager.UpdateAsync(user);
                if (userUpdate.Succeeded)
                {
                    StatusMessage = "Đã cập nhật user " + user.UserName +" thành công";
                }
                else
                {
                    StatusMessage = "Error: ";
                    foreach (var er in userUpdate.Errors)
                    {
                        StatusMessage += er.Description;
                    }
                }
            }
            else
            {
                Input.UserName = user.UserName;
                Input.FullName = user.FullName;
                Input.CMND = user.CMND;
                Input.PhoneNumber = user.PhoneNumber;
                IsConfirmed = true;
            }
            return Page();
        }
    }
}
