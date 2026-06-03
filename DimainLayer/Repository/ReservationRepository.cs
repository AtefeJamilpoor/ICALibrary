using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Contex;
using DimainLayer.Enums;
using DimainLayer.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace DimainLayer.Repository
{
    public class ReservationRepository:IReservationRepository
    {
        private readonly LibraryDBContext _dbContext;
        public ReservationRepository(LibraryDBContext dBContext)
        {
                _dbContext = dBContext;
        }

        public async Task Create(Reservation reservation)
        {
            try
            {
                await _dbContext.Reservations.AddAsync(reservation);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // اینجا خطا را لاگ کن یا موقتاً در Debug ببین
                Console.WriteLine("Error saving reservation: " + ex.Message);
                throw; // دوباره پرتاب کن تا ببینی کجا خطا رخ می‌دهد
            }

        }

        public async Task Delete(Reservation reservation)
        {
            _dbContext.Reservations.Remove(reservation);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Reservation>> GetAll()
        {
            return await _dbContext.Reservations.ToListAsync();
        }

        public async Task<Reservation?> GetById(int id)
        {
            return await _dbContext.Reservations.Where(x=>x.Id==id).SingleOrDefaultAsync();
        }

        public async Task Update(Reservation reservation)
        {
             _dbContext.Reservations.Update(reservation);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<Reservation>> GetReservationByDate(DateTime date)
        {
            return await _dbContext.Reservations.Where(x=>x.ReserveDate.Date==date)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetUserReservation(int userId)
        {
            return await _dbContext.Reservations.Where(x=>x.UserId==userId).ToListAsync();
        }

        public async Task<bool> IsBookAvailable(int bookId)
        {
            return !await _dbContext.Reservations.AnyAsync(x => x.BookId == bookId && x.ReturnDate ==null);
        }

        public async Task<List<Reservation>> GetByBookId(int bookId)
        {
            return await _dbContext.Reservations.Where(x=>x.BookId==bookId).ToListAsync();
        }

        public async Task<(bool isBorrowed, bool isReserved,bool hasLate,DateTime accessDate,DateTime returnDate)> GetBookStatus(int bookId)
        {
            var reservations = await GetByBookId(bookId);
            DateTime borrowD=DateTime.MinValue;
            DateTime reserveD = DateTime.MinValue;
            DateTime returnDate;
            DateTime accessDate=DateTime.Now;
             bool isBorrowed =  reservations.Any(r => r.IsBorrowed && r.ReturnDate == null && r.DueDate > DateTime.Now);
            bool isReserved = reservations.Any(r => !r.IsBorrowed && r.ReservationStatus == ReservationStatus.Active);
            bool hasLate = reservations.Any(r=>r.IsBorrowed && r.ReturnDate == null && r.DueDate < DateTime.Now);
            if (!hasLate)
            {
                if (isBorrowed)
                {
                    List<Reservation> borrow = reservations.Where(r => r.IsBorrowed && r.ReturnDate == null && r.DueDate > DateTime.Now).ToList();
                    borrowD = borrow.Select(r => r.DueDate).SingleOrDefault();
                    accessDate = borrow.Select(r => r.ReserveDate).SingleOrDefault();
                }
                if (isReserved)
                {
                    List<Reservation> reserve = reservations.Where(r => !r.IsBorrowed && r.ReservationStatus == ReservationStatus.Active).ToList();
                    reserveD = reserve.Select(r => r.DueDate).SingleOrDefault();
                    accessDate = reserve.Select(r => r.ReserveDate).SingleOrDefault();
                }
                returnDate = borrowD > reserveD ? borrowD : reserveD;
            }
            else
            {
                List<Reservation> borrow = reservations.Where(r => r.IsBorrowed && r.ReturnDate == null && r.DueDate < DateTime.Now).ToList();
                returnDate = borrow.Select(r => r.DueDate).SingleOrDefault();
                accessDate = borrow.Select(r=>r.ReserveDate).SingleOrDefault();
            }
            return (isBorrowed, isReserved, hasLate, accessDate, returnDate);
        }

        public async Task<int> CountBorrowedBooks(int userId)
        {
            return await _dbContext.Reservations
                .Where(r => r.UserId == userId && r.ReservationStatus == ReservationStatus.Borrowed)
                .CountAsync();
        }
        
        public async Task<int> CountReservedBooks(int userId)
        {
            return await _dbContext.Reservations
                .Where(r => r.UserId == userId && !r.IsBorrowed && r.ReservationStatus == ReservationStatus.Active)
                .CountAsync();
        }

        public async Task<Reservation> FindBorrowedByBookId(int bookId)
        {
            return await _dbContext.Reservations.FirstOrDefaultAsync( r =>r.BookId==bookId && r.IsBorrowed && r.ReturnDate == null);
        }

        public async Task<Reservation> FindBorrowedBook(int bookId, int userId)
        {
            return await _dbContext.Reservations
                .FirstOrDefaultAsync(r=>r.UserId==userId && r.BookId==bookId && r.ReturnDate==null
                && r.ReservationStatus== ReservationStatus.Borrowed && r.IsBorrowed==true); 
        }

        public async Task<Reservation> FindReservedBook(int bookId, int userId)
        {
            return await _dbContext.Reservations
                .FirstOrDefaultAsync(r=>r.ReturnDate==null 
                && r.ReservationStatus==ReservationStatus.Active && !r.IsBorrowed);
        }

        public async Task<List<int>> FindOverdueUsers()
        {
                List<int> overdueUsers = await _dbContext.Reservations
                .Where(r=>r.IsBorrowed && r.ReturnDate==null && r.DueDate<DateTime.Now)
                .Select(r=>r.UserId).Distinct().ToListAsync();
            return  overdueUsers;
        }

        public async Task<List<int>> FindOverdueUsersReservations()
        {
            List<int> overdueUsers = await _dbContext.Reservations
                .Where(r => !r.IsBorrowed && r.ReservationStatus == ReservationStatus.Active
                && r.DueDate < DateTime.Now).Select(r => r.UserId).Distinct().ToListAsync();
            return overdueUsers;
        }

        public async Task MakeExpiered(List<int> overDueUsers)
        {
            List<Reservation> reserves = await GetAll();
            List<Reservation> reservationsToCancel =  reserves
                .Where(r=>overDueUsers.Contains(r.UserId) && r.ReservationStatus==ReservationStatus.Active).ToList();
            foreach (var res in reservationsToCancel)
            {
                res.ReservationStatus = ReservationStatus.Expired;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Reservation>> GetByDate(DateTime date)
        {
           var GetReservationByDate = await _dbContext.Reservations
                .Where(r=>r.ReserveDate.Year==date.Year && r.ReserveDate.Month == date.Month && r.ReserveDate.Day == date.Day).ToListAsync();
            return GetReservationByDate;
        }

        public  bool IsAnyReserva(DateTime date)
        {
            bool isAnyReserva =  _dbContext.Reservations
                           .Any(r=>r.ReserveDate.Year == date.Year && r.ReserveDate.Month == date.Month && r.ReserveDate.Day==date.Day);
            return isAnyReserva;
        }
    }

   

        //public async Task<bool> UserHasLateReturn(int userId)
        //{
        //    var nowUtc = DateTime.UtcNow;
        //    return await _dbContext.Reservations.AnyAsync(x=>x.UserId==userId 
        //    &&(
        //    (!x.ReturnDate.HasValue && x.DueDate <nowUtc)
        //    ||
        //    (x.ReturnDate.HasValue && x.ReturnDate.Value>x.DueDate)
        //    ));
        //}
        //}
    }
