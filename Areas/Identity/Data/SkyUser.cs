using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Sky.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the SkyUser class
    public class SkyUser : IdentityUser
    {
        [PersonalData]
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Full Name")]
        [Column(TypeName = "nvarchar(100)")]

        public string FullName { get; set; }

        [PersonalData]
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "CMND")]
        [Column(TypeName = "nvarchar(12)")]
        public string CMND { get; set; }

        [PersonalData]
        [DataType(DataType.Text)]
        //[Display(Name = "")]
        [Column(TypeName = "nvarchar(12)")]
        public string UserRole { get; set; }
    }
}
