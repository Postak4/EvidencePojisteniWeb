using System.ComponentModel.DataAnnotations;

namespace EvidencePojisteniWeb.ValidationAttributes
{
    /// <summary>
    /// Validuje, že datum není starší než dnešní den - 
    /// datum je větší než attribut.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _otherPropertyName;

        public DateGreaterThanAttribute(string otherPropertyName)
        {
            _otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            var otherProp = context.ObjectType.GetProperty(_otherPropertyName);
            if (otherProp == null)
            {
                return new ValidationResult($"Nelze najít vlastnost '{_otherPropertyName}'.");
            }

            var otherValue = otherProp.GetValue(context.ObjectInstance);
            if (value is DateTime thisDate && otherValue is DateTime otherDate)
            {
                if (thisDate.Date <= otherDate.Date)
                {
                    return new ValidationResult(
                        ErrorMessage
                        ?? $"{context.DisplayName} musí být po datu {_otherPropertyName}.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
