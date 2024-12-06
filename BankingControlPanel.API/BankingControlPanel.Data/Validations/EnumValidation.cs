using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Data.Validations
{
    // Custom validation attribute to validate enum values for Sex and AccountType
    public class EnumValidation : ValidationAttribute
    {
        private readonly Type _enumType;
        // Constructor that accepts the enum type to validate
        public EnumValidation(Type enumType)
        {
            _enumType = enumType;
        }
        // Override the IsValid method to perform the validation
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // If the value is null, validation is considered successful
            if (value == null)
            {
                return ValidationResult.Success;
            }
            // Check if the provided value is defined in the specified enum type
            if (!Enum.IsDefined(_enumType, value))
            {
                // Return a validation error if the value is not a valid enum value
                return new ValidationResult($"The value '{value}' is not a valid enum value for {_enumType.Name}.");
            }
            // Return success if the value is valid
            return ValidationResult.Success;
        }
    }
}
 

