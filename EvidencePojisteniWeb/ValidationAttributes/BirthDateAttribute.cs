using System.ComponentModel.DataAnnotations;

namespace EvidencePojisteniWeb.ValidationAttributes
{
    public class BirthDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object value, ValidationContext context)
        {
            if (value is DateTime date)
            {
                if (date > DateTime.Today)
                    return new ValidationResult("Datum narození nemůže být v budoucnosti.");
                int age = DateTime.Today.Year - date.Year;
                if (date.Date > DateTime.Today.AddYears(-age)) age--;
                if (age > 120)
                    return new ValidationResult("Věk nesmí být vyšší než 120 let.");
                return ValidationResult.Success;
            }
            return new ValidationResult("Neplatné datum narození.");
        }
    }
}
