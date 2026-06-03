using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICALibrary.Models.Attribute;

namespace ICALibrary.Models.DTO
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "نام و نام خانوادگی اجباری است")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "کد ملی را وارد نکردید")]
        [MaxLength(10, ErrorMessage = "کد ملی بیشتر از 10 کاراکتر وارد شده.")]
        public string NationalCode { get; set; }

        [Required(ErrorMessage = "شماره تلفن  وارد نشده")]
        [MaxLength(15, ErrorMessage = "شماره تلفن بیشتر از 15 کاراکتر وارد شده")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "تصویر الزامی است")]
       // [ImageFile( new string[]{".jpg", ".jpeg",".png",".gif" })]
        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "وارد کردن ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر را وارد کنید")]
        public string Email { get; set; }

        [Required(ErrorMessage = "تاریخ تولد الزامی است")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}
