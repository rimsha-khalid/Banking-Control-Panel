using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
 

namespace BankingControlPanel.Data.Validations 
{
    // Custom validation attribute to validate mobile phone numbers
    public class MobileNumberValidation : ValidationAttribute
    {
        // Hardcoding only Pakistans country code for validation
        private const string countryCode = "PK";
        
        public MobileNumberValidation() { }


        // Override the IsValid method to perform the validation
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // If the value is null, validation is considered successful  
            if (value == null)
            {
                return ValidationResult.Success;  // Allow null values
            }

            // Convert the value to a string for processing
            var mobileNumber = value.ToString();

            try
            {
                // Create an instance of PhoneNumberUtil for parsing and validating phone numbers
                PhoneNumberUtil mobileNumberUtil = PhoneNumberUtil.GetInstance();

                // Parse the mobile number using Pakistan's country code
                PhoneNumber number = mobileNumberUtil.Parse(mobileNumber, countryCode);

                // Check if the parsed number is valid
                if (mobileNumberUtil.IsValidNumber(number))
                {
                    return ValidationResult.Success; // Return success if the number is valid
                }
                else
                {
                    return new ValidationResult("Invalid Pakistani phone number."); // Return error if the number is invalid
                }
            }
            catch (NumberParseException)
            {
                return new ValidationResult("Invalid phone number format."); // Return error if the phone number format is invalid
            }
        }
    }
} 
 