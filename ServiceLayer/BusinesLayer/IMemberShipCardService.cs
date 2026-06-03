using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;

namespace ServiceLayer.BusinesLayer
{
    public interface IMemberShipCardService
    {
        public Task<MembershipCard?> GetById(int id);
        public Task<MembershipCard> GenerateMembershipCardAsync(User user);
        public Task<MembershipCard?> GetByUserId(int userId);
        public Task<MembershipCard> RenewCard(int UserId);
    }
}
