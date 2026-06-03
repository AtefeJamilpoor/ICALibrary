using System.ComponentModel.DataAnnotations;
using DimainLayer.Enums;
using ICALibrary.Models.Attribute;
using ICALibrary.Models.Enum;

namespace ServiceLayer.DTO
{
    public class EditUserDTO
    {
        public int Id { get; set; } 
        [MaxLength(100, ErrorMessage = "نام و نام خانوادگی نباید بیشتر از 100 کاراکتر باشد")]
        public string? FullName { get; set; }

        [MaxLength(15, ErrorMessage = "شماره تلفن نباید بیشتر از 15 کاراکتر باشد")]
        [MinLength(11,ErrorMessage= "شماره تلفن نباید کمتر از 11 کاراکتر باشد")]
        [Phone(ErrorMessage = "شماره تلفن معتبر وارد کنید")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "ایمیل معتبر وارد کنید")]
        public string? Email { get; set; }

        
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        //[Required(ErrorMessage = "وضعیت کاربر الزامی است")]
        //[EnumDataType(typeof(UserEditStatus),ErrorMessage ="وضعیت کاربر را می توانید فقط فعال یا مسدود شده در نظر بگیرید")]
        //public UserEditStatus? Status { get; set; } = UserEditStatus.Active;

        [ImageFile(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        public IFormFile? Image { get; set; }
    }
}
