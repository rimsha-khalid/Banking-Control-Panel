using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Model.Models
{
    public class Registration
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Email must be in abc@example.com.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*])[A-Za-z\\d!@#$%^&*]{8,}$", ErrorMessage = "Password must contain 1 UpperCase letter, 1 LowerCase letter, 1 Numeric, 1 Spceial charater, and must be 8 digits long.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Password confirmation is required.")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
         
        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "User", Text = "User" },
            new SelectListItem { Value = "Admin", Text = "Admin" }
        };

    }
}
