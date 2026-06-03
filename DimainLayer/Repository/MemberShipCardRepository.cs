using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Contex;
using DimainLayer.Enums;
using DimainLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace DimainLayer.Repository
{
    public class MemberShipCardRepository:IMemberShipCardRepository
    {
        private readonly LibraryDBContext _dbContext;

        public MemberShipCardRepository(LibraryDBContext dBContext)
        {
                _dbContext = dBContext;
        }

        public async Task Create(MembershipCard membershipCard)
        {
            await _dbContext.MembershipCards.AddAsync(membershipCard);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(MembershipCard membershipCard)
        {
            _dbContext.MembershipCards.Remove(membershipCard);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<MembershipCard>> GetAll()
        {
            return await _dbContext.MembershipCards.ToListAsync();
        }

        public async Task<MembershipCard?> GetById(int id)
        {
            return await _dbContext.MembershipCards.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Update(MembershipCard membershipCard)
        {
            _dbContext.MembershipCards.Update(membershipCard);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<MembershipCard>> GetActiveCards()
        {
            return await _dbContext.MembershipCards
                .Where(x => !x.ExpiryDate.HasValue || x.ExpiryDate > DateTime.UtcNow).ToListAsync();
        }

        public async Task<MembershipCard?> GetByUserId(int userId)
        {
            MembershipCard? card = await _dbContext.MembershipCards.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if(card!= null)
            {
                if (card.ExpiryDate < DateTime.Now && card.Status==Enums.CardStatus.Active)
                {
                    card.Status = Enums.CardStatus.Expired;
                    await Update(card);
                }
            }
            return card;
        }

        public async Task<bool> ExistsByCardNumberAsync(string cardNumber)
        {
            return await _dbContext.MembershipCards.AnyAsync(x=>x.CardNumber==cardNumber);
        }

        public async Task<List<MembershipCard>> GetExpiredCards()
        {
            return await _dbContext.MembershipCards
                .Where(x => x.ExpiryDate.HasValue && x.ExpiryDate <= DateTime.UtcNow).ToListAsync();
        }

        public async Task<CardStatus> CardStatus(int cardId)
        {
            return await _dbContext.MembershipCards.Where(x => x.Id == cardId).Select(x=>x.Status).SingleOrDefaultAsync();
        }

        public async Task<MembershipCard> RenewCard(int UserId)
        {
            MembershipCard card = await _dbContext.MembershipCards.FirstAsync(x=>x.UserId == UserId);
            card.Status=Enums.CardStatus.Active;
            card.ExpiryDate = DateTime.Now.AddYears(1);
            await Update(card);
            return card;
        }

        public async Task<List<int>> FindUsersOverdueCards()
        {
            List<int> overDueUsers = await _dbContext.MembershipCards.Where(c=>c.ExpiryDate<DateTime.Now).Select(c=>c.UserId).Distinct().ToListAsync();   
            return overDueUsers;
        }
    }
}
