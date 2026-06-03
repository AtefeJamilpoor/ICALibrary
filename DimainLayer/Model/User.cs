using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Enums;

namespace DimainLayer.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="نام و نام خانوادگی را وارد نکردید")]
        [MaxLength(100,ErrorMessage ="نام خانوادگی بیشتر از 100 کاراکتر شده")]
        public string FullName {  get; set; }

        [Required(ErrorMessage ="کد ملی را وارد نکردید")]
        [MaxLength(10,ErrorMessage ="کد ملی بیشتر از 10 کاراکتر وارد شده.")]
        public string NationalCode { get; set; }

        [Required(ErrorMessage ="شماره تلفن  وارد نشده")]
        [MaxLength(15,ErrorMessage = "شماره تلفن بیشتر از 15 کاراکتر وارد شده")]
        public string PhoneNumber { get; set; }

        public string PhotoUrl { get; set; }

        //شماره عضویت یکتا
        [Required]
        public string MembershipNumber { get; set; }

        //تاریخ تولد
        [Required(ErrorMessage = "تاریخ تولد الزامی است")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required]
        public UserStatus Status { get; set; } = UserStatus.Active;

        //تاریخ عضویت
        [Required]
        public DateTime MemberShipDate { get; set; }= DateTime.Now;

        [Required(ErrorMessage = "وارد کردن ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر را وارد کنید")]
        public string Email { get; set; }

        //ارتباط یک به یک با کارت عضویت
        public MembershipCard MembershipCard { get; set; }

        //ارتباط یک به چند با رزروها
        public ICollection<Reservation> Reservation { get; set; } =new List<Reservation>();
    }
}
