using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sky.Areas.Identity.Data;

namespace Sky.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<SkyUser> _userManager;
        private readonly SignInManager<SkyUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;



        public LoginModel(SignInManager<SkyUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<SkyUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        [TempData]
        public string Status { set; get; }

        public class InputModel
        {
            [Required(ErrorMessage = "Bạn chưa nhập Email")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Bạn chưa nhập mật khẩu")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            Status = "";

            if (returnUrl != null)
            {
                Status = "Bạn cần đăng nhập để tiếp tục!";
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (returnUrl == null)
            {
                Status = "";
            }
            else
            {
                Status = "Bạn cần đăng nhập để tiếp tục";
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                  
                // This doesn't count login failures towards account lockout
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if(user.LockoutEnabled == false)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa");
                    return Page();
                }

                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    if (await _userManager.IsInRoleAsync(user, "Admin") || await _userManager.IsInRoleAsync(user, "Manager"))
                        return LocalRedirect("~/admin");

                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Tài khoản của bạn đã bị khóa");
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa");
                    //return RedirectToPage("./Lockout");
                    return Page();
                }
                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn chưa xác nhận Email");
                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Thông tin đăng nhập sai");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
