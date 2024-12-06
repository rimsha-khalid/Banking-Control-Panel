using BankingControlPanel.Data.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankingControlPanel.Data.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; } // Unique identifier for the client

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Email must be in abc@example.com")]
        public string Email { get; set; } // Required email address of the client, must match a specific format

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(59, ErrorMessage = "First name should be less than 60 characters")]
        public string? FirstName { get; set; } // Required first name of the client, must be less than 60 characters
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(59, ErrorMessage = "Last name should be less than 60 characters")]
        public string? LastName { get; set; } // Required last name of the client, must be less than 60 characters
        [Required(ErrorMessage = "Perosnal Id is required.")]
        [RegularExpression("^\\d{11}$", ErrorMessage = "Peronal Id should be exactly 11 characters")]
        public string? PersonalId { get; set; } // Required personal Id of the client, must be exactly 11 digits
        public string? ProfilePhoto {  get; set; }  // Represents profile photo URL or path

        [MobileNumberValidation(ErrorMessage = "Invalid phone number format. Ensure you use the correct format like +92 or 0092.")]
        public string MobileNumber { get; set; } // Represents mobile number of the client, must follow a specific format

        [Required(ErrorMessage = "Sex is required.")]
        [EnumValidation(typeof(Sex))]
        public Sex Sex { get; set; } // Required sex of the client, must be one of the defined enum values
        public bool IsActive { get; set; } // Indicates if the client is active
        public virtual Address Address { get; set; }  // Navigation property of Clients's Address

        [ForeignKey("UserId")]
        public int? UserId { get; set; }   // Foreign Key of associated User
        
        [JsonIgnore]
        public virtual User? User { get; set; } // Navigation property for assocaited User and ignored during JSON serialization

        public virtual ICollection<Account>? Account { get; set; } // Navigation Property for Client's Account

    }
    public enum Sex
    {
        Male,
        Female
    }
}
