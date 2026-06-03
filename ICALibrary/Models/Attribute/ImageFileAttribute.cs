using System.ComponentModel.DataAnnotations;

namespace ICALibrary.Models.Attribute
{
    public class ImageFileAttribute:ValidationAttribute
    {
        private readonly string[] _expection;
        public ImageFileAttribute(string[] expection)
        {
                _expection = expection;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null) 
            { 
                var exeption = Path.GetFileName(file.FileName);
                if (!_expection.Contains(exeption))
                {
                    return new ValidationResult($"{string.Join(",",_expection)}");
                }
            }
            return ValidationResult.Success;
        }
    }
}
