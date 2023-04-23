using System.ComponentModel.DataAnnotations;

namespace ATLManager.Areas.Identity.Pages.Account
{
    public static class ValidationHelper
    {
        public static ValidationResult ValidateBirthDate(DateTime birthDate, ValidationContext context)
        {
            if (birthDate.Date.AddYears(18) > DateTime.Today)
            {
                return new ValidationResult("Você deve ter pelo menos 18 anos para se registar como administrador.");
            }

            return ValidationResult.Success;
        }
    }

}
