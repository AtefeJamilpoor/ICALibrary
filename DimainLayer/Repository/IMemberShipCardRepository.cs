using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Enums;
using DimainLayer.Model;

namespace DimainLayer.Repository
{
    public interface IMemberShipCardRepository
    {
        //متدهای پایه
        public Task Create(MembershipCard membershipCard);
        public Task Delete(MembershipCard membershipCard);
        public Task<List<MembershipCard>> GetAll();
        public Task<MembershipCard?> GetById(int id);
        public Task Update(MembershipCard membershipCard);

        //متدهای اختصاصی

        //کارت کاربر خاص
        public Task<MembershipCard?> GetByUserId(int userId);
        //کارت های فعال
        public Task<List<MembershipCard>> GetActiveCards();
        //کارت های منقضی شده
        public Task<List<MembershipCard>> GetExpiredCards();
        //بررسی فعال بودن کارت
        public Task<CardStatus> CardStatus(int cardId);
        public  Task<bool> ExistsByCardNumberAsync(string cardNumber);
        public Task<MembershipCard> RenewCard(int UserId);
        public Task<List<int>> FindUsersOverdueCards();
    }
}
