using DimainLayer.Model;
using ICALibrary.Models.DTO;

namespace ICALibrary.Models.ViewModel
{
    public class UserDetailsViewModel
    {
        public User? User { get; set; }
        public MembershipCard? MembershipCard { get; set; } // ممکنه null باشه
    }
}
