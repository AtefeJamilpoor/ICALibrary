using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;
using ServiceLayer.DTO;

namespace ServiceLayer.BusinesLayer
{

    public interface IUserService
    {
        public Task CreateUser(CreateUserDTOLayer dto);
        public  Task<bool> UserExistsByNationalCodeAsync(string nationalCode);
        public Task<List<User?>> GetAllUsers();
        public  Task<User?> GetById(int id);
        public List<UserReportDto> GetUsersReport();
        public Task Update(User user);
    }
}
