using System.ComponentModel.DataAnnotations;

namespace EvidencePojisteniWeb.ValidationAttributes
{
    /// <summary>
    /// Validuje, že datum nesmí být v budoucnosti (smí být maximálně dnešní den)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is DateTime dt && dt.Date > DateTime.Today)
            {
                // Vracíme vlastní chybovou zprávu nebo defaultní ErrorMessage
                var error = string.IsNullOrEmpty(ErrorMessage)
                    ? "Datum nesmí být v budoucnosti."
                    : ErrorMessage;
                return new ValidationResult(error, new[] { context.MemberName! });
            }

            // Pokud je validace úspěšná, vrátíme ValidationResult.Success
            return ValidationResult.Success;
        }
    }
}
