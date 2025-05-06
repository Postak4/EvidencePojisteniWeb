using System.ComponentModel.DataAnnotations;

namespace EvidencePojisteniWeb.ValidationAttributes
{
    /// <summary>
    /// Validuje, že datum není starší než dnešní den.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateNotInPastAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is DateTime dt && dt.Date < DateTime.Today)
            {
                // Vracíme vlastní chybovou zprávu nebo defaultní ErrorMessage
                var error = string.IsNullOrEmpty(ErrorMessage)
                    ? "Datum nesmí být v minulosti."
                    : ErrorMessage;
                return new ValidationResult(error, new[] { context.MemberName! });
            }

            // Pokud je validace úspěšná, vrátíme ValidationResult.Success
            return ValidationResult.Success;
        }
    }
}
