using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sky.Areas.Identity.Data;

namespace Sky.Areas.Identity.Pages
{
    [Authorize(Roles = "Admin, Manager")]
    public class CarouselModel : PageModel
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public CarouselModel(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        /// Dùng session để lưu
        public string StatusMessage { set; get; }

        public List<string> AllImage { set; get; } = new List<string>();

        public class InputModel
        {
            [Required(ErrorMessage = "Bạn chưa chọn file")]
            [Display(Name = "Chọn file")]
            public IFormFile ImageFile { get; set; }

            public string ImageName { set; get;}

        }

        public void GetAllImage()
        {
            string path = Path.Combine(_hostEnvironment.WebRootPath, "Panel");

            string[] filePaths1 = Directory.GetFiles(path, "*.png");

            string[] filePaths2 = Directory.GetFiles(path, "*.jpg");

            string[] filePaths3 = Directory.GetFiles(path, "*.jpeg");

            var filePaths = filePaths1.Union(filePaths2).Union(filePaths3).ToArray();

            foreach (var i in filePaths)
            {
                string temp = Path.GetFileName(i);
                AllImage.Add(temp);
            }
        }

        [BindProperty]
        public InputModel Input { set; get; }
        public IActionResult OnGet()
        {
            GetAllImage();
            ModelState.Clear();
            return Page();
        }

        public IActionResult OnPost() => NotFound(); 
        public async Task<IActionResult> OnPostAddImage()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(Input.ImageFile.FileName);
                    string extension = Path.GetExtension(Input.ImageFile.FileName);
                    fileName = "Panel" + DateTime.Now.ToString("ddMMyyyyHHmmssff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Panel/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await Input.ImageFile.CopyToAsync(fileStream);
                    }
                    StatusMessage = "Thêm thành công";

                    GetAllImage();
                }
                catch
                {
                    StatusMessage = "Thêm không thành công";
                }
            }
            else
            {
                StatusMessage = null;
            }

            return Page();
        }
        public IActionResult OnPostDeleteImage()
        {
            if (Input.ImageName != null)
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "panel", Input.ImageName);

                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);

                StatusMessage = "Đã xóa";
            }

            GetAllImage();
            ModelState.Clear();

            return Page();
        }
    }
}
