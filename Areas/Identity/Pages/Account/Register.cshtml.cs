using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Sky.Areas.Identity.Data;

namespace Sky.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<SkyUser> _signInManager;
        private readonly UserManager<SkyUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        public RegisterModel(
            UserManager<SkyUser> userManager,
            SignInManager<SkyUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Bạn phải nhập tên")]
            [DataType(DataType.Text)]
            [Display(Name = "Tên đầy đủ")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Bạn cần nhập CMND")]
            [DataType(DataType.Text)]
            [Display(Name = "CMND")]
            public string CMND { get; set; }

            [Required]
            [Display(Name = "Địa chỉ Email")]
            [EmailAddress(ErrorMessage = "Đây không phải là email hợp lệ")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Mật khẩu tối thiểu 6 kí tự", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Nhập lại mật khẩu")]
            [Compare("Password", ErrorMessage = "2 mật khẩu phải giống nhau")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Home");
            }
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
            if (ModelState.IsValid)
            {
                //for (int i = 0; i < 30; i++)
                //{
                //    await _userManager.CreateAsync(new SkyUser { UserName = "user" + i.ToString(), Email = "user" + i.ToString() + "@gmail.com", FullName = "user" + i.ToString(), CMND = i.ToString() }, "123456");
                //}
                var user = new SkyUser { UserName = Input.Email, Email = Input.Email, FullName = Input.FullName, CMND = Input.CMND };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.AddToRoleAsync(user, "User");
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code, returnUrl },
                        protocol: Request.Scheme);

                    //var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { code, email = user.Email }, Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


                    //Send mail by SMTP 
                    //_sendMail.SendMailOutlook(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    System.Net.NetworkCredential credential = new System.Net.NetworkCredential("meow.tnv@outlook.com", "Meow1234@");
                    SmtpClient client = new SmtpClient("smtp-mail.outlook.com")
                    {
                        Port = 587,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        EnableSsl = true,
                        Credentials = credential
                    };
                    MailMessage message = new MailMessage("meow.tnv@outlook.com", Input.Email)
                    {
                        Subject = "Xác nhận Email",
                        Body = "<img src='https://i.imgur.com/fs3LmWO.png'> <br>"
                            + $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                        IsBodyHtml = true
                    };
                    try
                    {
                        client.Send(message);
                    }
                    catch {
                        ModelState.AddModelError(string.Empty, "Đã có lỗi khi gửi mail xác nhận, liên hệ admin để được xác nhận!");
                        return Page();
                    }
                    
                    //


                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
