using DimainLayer.Enums;

namespace ICALibrary.Models.ViewModel
{
    public class MembershipCardViewModel
    {
        public string UserFullName { get; set; }
        public string MembershipNumber { get; set; }
        public string NationalCode { get; set; }
        public string PhotoUrl { get; set; }

        public string CardNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public CardStatus Status { get; set; }

    }
}
