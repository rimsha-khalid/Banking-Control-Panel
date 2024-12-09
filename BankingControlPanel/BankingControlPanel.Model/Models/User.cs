﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Model.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; } // Unique identifer for user

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Email must be in abc@example.com.")]
        public string Email { get; set; } // Represents user's email

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*])[A-Za-z\\d!@#$%^&*]{8,}$", ErrorMessage = "Password must contain 1 UpperCase letter, 1 LowerCase letter, 1 Numeric, 1 Spceial charater, and must be 8 digits long.")]
        public string Password { get; set; } // Represents user's password

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } // Represents user's role (Admin or User)
 
    }
}
