using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Validation
{
    public class ExpirationDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string expirationDate)
            {
                var expirationDateParts = expirationDate.Split('/');
                var expirationMonth = int.Parse(expirationDateParts[0]);
                var expirationYear = int.Parse(expirationDateParts[1]);
                var currentMonth = DateTime.UtcNow.Month;
                var currentYear = DateTime.UtcNow.Year % 100;

                if (expirationYear > currentYear)
                {
                    return true;
                }
                else if (expirationYear == currentYear)
                {
                    return expirationMonth > currentMonth;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
