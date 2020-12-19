using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sky.Areas.Identity.Pages.Role
{
    [Authorize(Roles = "Manager")]
    public class DeleteModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public DeleteModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public class InputModel
        {
            [Required]
            public string ID { set; get; }
            public string Name { set; get; }

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

            var role = await _roleManager.FindByIdAsync(Input.ID);
            if (role == null)
            {
                return NotFound("Không thấy role cần xóa");
            }

            ModelState.Clear();

            if (IsConfirmed)
            {
                //Xóa
                await _roleManager.DeleteAsync(role);
                StatusMessage = "Đã xóa " + role.Name;

                return RedirectToPage("Index");
            }
            else
            {
                Input.Name = role.Name;
                IsConfirmed = true;

            }

            return Page();
        }
    }
}
