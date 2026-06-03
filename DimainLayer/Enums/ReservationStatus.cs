using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimainLayer.Enums
{
    public enum ReservationStatus
    {
        Active,      // رزرو فعال
        Cancelled,   // رزرو لغو شده
        Returned,   // کتاب بازگردانده شده
        Overdue,     // دیرکرد در بازگشت
        Expired,     // اعتبار رزرو تمام شده (بعد از 3 روز از DueDate)
        Borrowed,
    }
}
