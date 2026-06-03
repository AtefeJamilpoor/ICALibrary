using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;
using DimainLayer.Repository;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.DTO;

namespace ServiceLayer.BusinesLayer
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateUser(CreateUserDTOLayer dto)
        {
            var membershipNumber = await _userRepository.GenerateUniqMembershipNumber();

            var user = new User
            {
                FullName = dto.FullName,
                NationalCode = dto.NationalCode,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                BirthDate = dto.BirthDate,
                MembershipNumber = membershipNumber,
                PhotoUrl = dto.ImagePath
            };

            await _userRepository.Create(user);

        }

        public async Task<List<User?>> GetAllUsers()
        {
            var users =await  _userRepository.GetAll();
            return users;
        }

        public async Task<User?> GetById(int id)
        {
            return await _userRepository.GetById(id);
        }

        public List<UserReportDto> GetUsersReport()
        {
            var users = _userRepository.GetUsersReport();
            var pc = new System.Globalization.PersianCalendar();

            return users.Select(u => new UserReportDto
            {
                FullName = u.FullName,
                NationalCode = u.NationalCode,
                MembershipNumber = u.MembershipNumber,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                BirthDate = $"{u.BirthDate.Year}/{u.BirthDate.Month:00}/{u.BirthDate.Day:00}",
                MembershipDate = $"{pc.GetYear(u.MemberShipDate)}/{pc.GetMonth(u.MemberShipDate):00}/{pc.GetDayOfMonth(u.MemberShipDate):00}",
                Status = u.Status.ToString(),
            }).ToList();

        }

        public async Task<bool> UserExistsByNationalCodeAsync(string nationalCode)
        {
            var user = await _userRepository.GetByNationalCode(nationalCode);
            return user != null;
        }

        public async Task Update(User user)
        {
             await _userRepository.Update(user);
        }

    }
}
