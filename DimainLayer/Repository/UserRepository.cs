using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Contex;
using DimainLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace DimainLayer.Repository
{
    public class UserRepository:IUserRepository
    {
        private readonly LibraryDBContext _dbContext;
        public UserRepository(LibraryDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        // متد تولید کد یکتا برای هر کاربر
        public async Task<string> GenerateUniqMembershipNumber()
        {
            string membershipNumber;
            bool exists;
            do
            {
                membershipNumber = $"U{DateTime.Now:yyyyMMddHHmmss}" +
                    $"{Guid.NewGuid().ToString().Substring(0, 4)}";
                exists = await _dbContext.Users.AnyAsync(x => x.MembershipNumber == membershipNumber);
            } while (exists);
            return membershipNumber;
        }
        public async Task Create(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(User user)
        {
            _dbContext.Users.Remove(user);
             await _dbContext.SaveChangesAsync();
        }

        public async Task<List<User>> GetAll()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetById(int id)
        {
            return await _dbContext.Users.Where(x=>x.Id == id).FirstOrDefaultAsync();
        }
        public async Task<User?> GetMembershipNumber(string membershipNumber)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u=>u.MembershipNumber==membershipNumber);
        }

        public async Task Update(User user)
        {
            _dbContext.Users.Update(user);
             await _dbContext.SaveChangesAsync();
        }
        public async Task<User?> GetByNationalCode(string nationalCode)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.NationalCode == nationalCode);
        }

        public  List<User?> GetUsersReport()
        {
            return  _dbContext.Users.ToList();
        }
    }
}
