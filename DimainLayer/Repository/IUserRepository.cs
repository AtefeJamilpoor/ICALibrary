using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;

namespace DimainLayer.Repository
{
    public interface IUserRepository
    {
        public Task Create(User user);
        public Task Delete(User user);
        public Task<List<User>> GetAll();
        public Task<User> GetById(int id);
        public Task Update(User user);
        public Task<string> GenerateUniqMembershipNumber();
        public  Task<User?> GetByNationalCode(string nationalCode);
        public List<User?> GetUsersReport();
    }
}
