using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BankingControlPanel.Model.Models
{
    public class ClientRegistration
    {
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Email must be in abc@example.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(59, ErrorMessage = "First name should be less than 60 characters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(59, ErrorMessage = "Last name should be less than 60 characters")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Perosnal Id is required.")]
        [RegularExpression("^\\d{11}$", ErrorMessage = "Peronal Id should be exactly 11 characters")]
        public string PersonalId { get; set; }
        public string? Path { get; set; }
        public IFormFile? ProfilePhoto { get; set; }

       
        [RegularExpression("^((\\+92)?(0092)?(92)?(0)?)(3)([0-9]{9})$", ErrorMessage = "Mobile Number should be in correct format")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Sex is required.")]
        public Sex Sex { get; set; }
        public string Country { get; set; }
        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }
        public string? Street { get; set; }
        public string? ZipCode { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public AccountType AccountType { get; set; }

    }
    
}

