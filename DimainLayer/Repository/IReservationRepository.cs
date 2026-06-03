using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DimainLayer.Repository
{
    public interface IReservationRepository
    {
        //متدهای پایه
        public Task Create(Reservation reservation);
        public Task Delete(Reservation reservation);
        public Task<List<Reservation>> GetAll();
        public Task<Reservation>GetById(int id);
        public Task Update(Reservation reservation);

        //متدهای اختصاصی پروژه

        //بررسی دیرکرد کاربر
        //public Task<bool> UserHasLateReturn(int userId);

        //گزارش رزروها براساس تاریخ
        public Task<List<Reservation>> GetReservationByDate(DateTime date);
        //بررسی آزاد بودن کتاب
        public Task<bool> IsBookAvailable(int bookId);
        //لیست رزروهای کاربر
        public Task<List<Reservation>> GetUserReservation(int userId);

        public Task<Reservation> FindBorrowedByBookId(int bookId);

        public Task<(bool isBorrowed, bool isReserved, bool hasLate, DateTime accessDate ,DateTime returnDate)> GetBookStatus(int bookId);
        public Task<int> CountBorrowedBooks(int userId);
        public Task<int> CountReservedBooks(int userId);
        public Task<Reservation> FindBorrowedBook(int bookId,int userId);
        public Task<Reservation> FindReservedBook(int bookId,int userId);
        public Task<List<int>> FindOverdueUsers();
        public Task<List<int>> FindOverdueUsersReservations();
        public Task MakeExpiered(List<int> overDueUsers);
        public Task<List<Reservation>> GetByDate(DateTime date);
        public bool IsAnyReserva(DateTime date);
        public Task<List<Reservation>> GetByBookId(int bookId);
    }
}
