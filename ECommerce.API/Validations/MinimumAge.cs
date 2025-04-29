using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace ECommerce.API.Validations
{
    public class MinimumAge(int minAge) : ValidationAttribute
    {

        private readonly int _minAge = minAge;
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateOfBirth)
            {
                if (DateTime.Now.Year - dateOfBirth.Year >= _minAge) return true;
            }
            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            return $"The age must be at least {_minAge} years old.";
        }
    }
}
