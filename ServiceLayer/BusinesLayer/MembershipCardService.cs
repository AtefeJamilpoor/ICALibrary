using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Enums;
using DimainLayer.Model;
using DimainLayer.Repository;

namespace ServiceLayer.BusinesLayer
{
    public class MembershipCardService : IMemberShipCardService
    {
        private readonly IMemberShipCardRepository _memberShipCardRepository;
        private readonly IUserRepository _userRepository;
        public MembershipCardService(IMemberShipCardRepository memberShipCardRepository,IUserRepository userRepository)
        {
            _memberShipCardRepository = memberShipCardRepository;
            _userRepository = userRepository;
        }
        public async Task<MembershipCard?> GetById(int id)
        {
            return await _memberShipCardRepository.GetById(id);
        }
        public async Task<MembershipCard> GenerateMembershipCardAsync(User user)
        {
            string cardNumber;
            do
            {
                cardNumber = "CARD-" + Guid.NewGuid().ToString().Substring(0, 8);
            }
            while (await _memberShipCardRepository.ExistsByCardNumberAsync(cardNumber));
            var card = new MembershipCard
            {
                CardNumber = cardNumber,
                IssueDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(1),
                PhotoUrl = user.PhotoUrl,
                Status = CardStatus.Active,
                UserId = user.Id
            };
            await _memberShipCardRepository.Create(card);
            user.MembershipCard = card;
            await _userRepository.Update(user);
            return card;
        }

        public async Task<MembershipCard?> GetByUserId(int userId)
        {
            return await _memberShipCardRepository.GetByUserId(userId);
        }
        public async Task<MembershipCard> RenewCard(int UserId)
        {
            return await _memberShipCardRepository.RenewCard(UserId);
        }
    }
}
