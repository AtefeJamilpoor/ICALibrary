namespace ServiceLayer.DTO;

public class UserReportDto
{
    public string FullName { get; set; }
    public string NationalCode { get; set; }
    public string MembershipNumber { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string BirthDate { get; set; }

    // تاریخ‌ها رو همونجا به شمسی تبدیل کن
    public string MembershipDate { get; set; }
    public string Status { get; set; }

}
