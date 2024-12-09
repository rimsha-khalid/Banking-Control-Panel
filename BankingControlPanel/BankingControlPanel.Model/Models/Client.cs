using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Model.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Email must be in abc@example.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(59, ErrorMessage = "First name should be less than 60 characters")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(59, ErrorMessage = "Last name should be less than 60 characters")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Perosnal Id is required.")]
        [RegularExpression("^\\d{11}$", ErrorMessage = "Peronal Id should be exactly 11 characters")]
        public string? PersonalId { get; set; }
        public string? ProfilePhoto { get; set; }

        //[RegularExpression("^((\\+92)|(0092))-{0,1}\\d{3}-{0,1}\\d{7}$|^\\d{11}$|^\\d{4}-\\d{7}$")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Sex is required.")]
        public Sex Sex { get; set; }
        public bool IsActive { get; set; }
        // Navigation property of Clients's Address
        public virtual Address Address { get; set; }
 
        // Foreign Key of associated User
        [ForeignKey("UserId")]
        public int? UserId { get; set; }
         
        // Navigation Property for Client's Account
        public virtual ICollection<Account>? Account { get; set; }

    }
    public enum Sex
    {
        Male,
        Female
    }
}

