﻿using System.ComponentModel.DataAnnotations;

namespace ATLManager.Attributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
			_maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
			if (value is IFormFile file)
			{
				if (file.Length > _maxFileSize)
				{
					return new ValidationResult(GetErrorMessage());
				}
			}

			return ValidationResult.Success;
		}

        public string GetErrorMessage()
        {
            return (ErrorMessage ?? $"O tamanho do ficheiro é superior ao máximo permitido");
        }
    }
}
