using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Validation
{
    public class GreaterThanZeroAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue > 0;
            }

            return false;
        }
    }
}
