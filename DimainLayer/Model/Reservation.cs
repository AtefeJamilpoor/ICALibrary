using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Enums;

namespace DimainLayer.Model
{
    public  class Reservation
    {
        [Key]
        public int Id { get; set; }

        //کلید خارجی به User
        [Required]
        [ForeignKey("User")]
        public int UserId {  get; set; }
        public User User { get; set; }

        //کلید خارجی به Book
        [Required]
        [ForeignKey("Book")]
        public int BookId {  get; set; }
        public Book Book { get; set; }

        //تاریخ رزرو
        [Required]
        public DateTime ReserveDate { get; set; }= DateTime.Now;

        //تاریخ موعد بازگشت
        [Required]
        public DateTime DueDate {  get; set; }

        //تاریخ بازگشت واقعی (اختیاری
        public DateTime ? ReturnDate { get; set; }

        //وضعیت رزرو
        [Required]
        public ReservationStatus ReservationStatus { get; set; }

        //اگر isborrowed = false باشد یعنی این کاربر این کتاب را رزرو کرده اما هنوز تحویل نگرفته
        public bool IsBorrowed { get; set; }
    }
}
