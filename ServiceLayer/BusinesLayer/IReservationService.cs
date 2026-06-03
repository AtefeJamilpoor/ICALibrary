using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;
using ServiceLayer.DTO;
using ServiceLayer.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServiceLayer.BusinesLayer
{
    public interface IReservationService
    {
        public Task<List<Reservation>> GetUserReservation(int userId);
        public Task Create(int bookId, int userId, bool isBorrowed);
        public Task<(bool isBorrowed, bool isReserved, bool hasLate, DateTime accessDate, DateTime returnDate)> GetBookStatus(int bookId);
        public Task<(List<BookInfoPartial> Borrowed, List<BookInfoPartial> Reserved, List<BookInfoPartial> Overdue, bool hasAvalableCard)> GetReservationInfo(int userId);
        public Task UpdateReserveToBorrow(int bookId, int userId);
        public Task BorrowToReturn(int bookId,int userId);
        public Task CancelReservation(int bookId,int userId);
        public Task<List<BookReservationViewModel>> ShowReservationsDate(DateTime date);
        public bool AnyReserva(DateTime date);
        Task<BookInfoPartial?> GetByISBN(string ISBN);
    }
}
