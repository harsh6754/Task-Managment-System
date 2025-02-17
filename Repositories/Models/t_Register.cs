using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models
{
    public class t_Register
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   
        public int c_userid {get; set;}

        [Required(ErrorMessage = "Please enter your username")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Username must start with a letter and can only contain letters and numbers.")]
        [Display(Name = "Username")]
        public string c_username {get; set;}

        [Required(ErrorMessage = "Please enter your Email")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email")]
        public string c_email {get; set;}

        [Required(ErrorMessage = "Please enter your password")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z0-9])(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]).{8,}$", ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter or number, and one special symbol.")]
        public string c_password {get; set;}

        [StringLength(100)]
        [Required(ErrorMessage ="enter confirm password")]  
        [Compare("c_password", ErrorMessage = "Password and Confirm Password must match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string c_confirmpassword {get; set;}

        [StringLength(100)]
        [Required(ErrorMessage ="enter your address")]
        [Display(Name = "Address")]
        public string c_address {get; set;}

        [StringLength(12, MinimumLength = 10, ErrorMessage = "Mobile number must be between 10 and 12 digits.")]
        [Required(ErrorMessage = "Enter your mobile number")]
        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^(\+\d{1,3}[- ]?)?\(?(\d{3})\)?[-. ]?(\d{3})[-. ]?(\d{4})$", ErrorMessage = "Invalid mobile number format.")]
        public string c_mobile { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage ="Choose gender")]
        [Display(Name = "Gender")]
        public string? c_gender { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime c_dob { get; set; }


       
        [StringLength(4000)]
        [Display(Name = "Profile Picture")]
        public string? c_image { get; set; }
         
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePic { get; set; }

        [Required(ErrorMessage = "Please select your country")]
        [Display(Name = "Country")]
        public string? c_country { get; set; }

        [Required(ErrorMessage = "Please select your state")]
        [Display(Name = "State")]
        public string? c_state { get; set; }

        [Required(ErrorMessage = "Please select your district")]
        [Display(Name = "District")]
        public string? c_district { get; set; }
    }
}