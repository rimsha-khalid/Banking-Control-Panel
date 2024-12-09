using BankingControlPanel.Data.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BankingControlPanel.Data.Models
{
    public class Account   // Represents a bank account
    {
        [Key]
        public int AccountId { get; set; } // Unique identifier for the account

        [Required (ErrorMessage = "Account number is required.")]
        [StringLength(16, MinimumLength = 12, ErrorMessage = "Account number must be between 12 to 16 digits.")]
        public string AccountNumber { get; set; }   // Represents account number
        public decimal Balance { get; set; }    // Represents current balance of the account
        [Required (ErrorMessage = "Account type is required.")]
        [EnumValidation(typeof(AccountType))]
        public AccountType AccountType { get; set; }  // Represents the type of account (Current or Saving)
        
        [ForeignKey("ClientId")] 
        public int ClientId { get; set; } // Foriegn Key of User's Client
        [JsonIgnore]
        public virtual Client? Client { get; set; } // Navigation property of User's Client and ignored during JSON serialization
    }
    public enum AccountType // Types of bank accounts 
    {
        Current, // Current Account
        Saving // Saving Account
    }
}
