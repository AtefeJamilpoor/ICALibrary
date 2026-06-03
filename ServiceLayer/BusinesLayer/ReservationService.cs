using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Enums;
using DimainLayer.Model;
using DimainLayer.Repository;
using Microsoft.VisualBasic;
using ServiceLayer.DTO;
using ServiceLayer.ViewModel;

namespace ServiceLayer.BusinesLayer
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberShipCardRepository _card;

        public ReservationService(IReservationRepository reservationRepository, IBookRepository bookRepository,IMemberShipCardRepository card)
        {
            _bookRepository = bookRepository;
            _reservationRepository = reservationRepository;
            _card = card;
        }

        public async Task Create(int bookId,int userId,bool isBorrowed)
        {
            Reservation model = new Reservation();

            model.UserId = userId;
            model.BookId = bookId;
            if (isBorrowed)
            {
                int borrowedCount = await _reservationRepository.CountBorrowedBooks(userId);

                if (borrowedCount >= 3)
                {
                    throw new InvalidOperationException("کاربر بیش از 3 کتاب را نمی تواند به امانت بگیرد.");
                }
                //یعنی تحویل داده شده
                model.IsBorrowed = true;
                model.DueDate= DateTime.Now.AddDays(14);
                model.ReservationStatus= ReservationStatus.Borrowed;

                Book book = await _bookRepository.GetById(bookId);

                if (book != null)
                {
                    book.BookStatus = BookStatus.Borrowed;
                    await _bookRepository.Update(book);
                }
            }
            else
            {
                int reservedCount = await _reservationRepository.CountReservedBooks(userId);

                if (reservedCount >= 3)
                {
                    throw new InvalidOperationException("کاربر بیش از 3 کتاب را نمی تواند به امانت بگیرد.");
                }
                model.IsBorrowed = false;
                Reservation r = await _reservationRepository.FindBorrowedByBookId(bookId);
                model.ReserveDate = r.DueDate.AddDays(1);
                model.DueDate = r.DueDate.AddDays(3);
                model.ReservationStatus = ReservationStatus.Active;
            }
            await  _reservationRepository.Create(model);
            
        }

        public async Task<(bool isBorrowed, bool isReserved, bool hasLate,DateTime accessDate, DateTime returnDate)> GetBookStatus(int bookId)
        {
            return await _reservationRepository.GetBookStatus(bookId);
        }

        public async Task<(List<BookInfoPartial> Borrowed, List<BookInfoPartial> Reserved, List<BookInfoPartial> Overdue, bool hasAvalableCard)> GetReservationInfo(int userId)
        {
            List<Reservation> reservation =await GetUserReservation(userId);
            List<Book> books = await _bookRepository.GetAll();
            MembershipCard? userCard =await _card.GetByUserId(userId);

            List<BookInfoPartial> Borrowed = reservation
               .Where(r => r.UserId == userId && r.IsBorrowed && r.ReturnDate == null && r.DueDate>DateTime.Now)
               .Join(books,r=>r.BookId,b=>b.Id,(r,b)=>new BookInfoPartial
               {
                   Book=b,
                   IsBorrowed=true,
                   IsReserver=false,
                   HasLate=false,
                   accessDate=r.ReserveDate,
                   returnDate = r.DueDate,
               })
               .ToList();

            List<BookInfoPartial> Reserved = reservation
                .Where(r => r.UserId == userId && !r.IsBorrowed && r.ReservationStatus == ReservationStatus.Active)
                .Join(books,r=>r.BookId, b=>b.Id,(r,b)=>new BookInfoPartial
                {
                    Book=b,
                    IsBorrowed=false,
                    IsReserver=true,
                    HasLate=false,
                    accessDate= r.ReserveDate,
                    returnDate = r.DueDate,
                })
                .ToList();

            List<BookInfoPartial> Overdue = reservation
            .Where(r => r.UserId == userId && r.IsBorrowed && r.ReturnDate == null && r.DueDate < DateTime.Now)
            .Join(books, r => r.BookId, b => b.Id, (r, b) => new BookInfoPartial
            {
                Book = b,
                IsBorrowed=false,
                IsReserver=false,
                HasLate=true,
                accessDate= r.ReserveDate,
                returnDate= r.DueDate,
            })
            .ToList();

            bool hasAvalableCard = userCard.ExpiryDate>DateTime.Now? true:false;

            return  (Borrowed, Reserved, Overdue, hasAvalableCard);
        }

        public async Task<List<Reservation>> GetUserReservation(int userId)
        {
            return await _reservationRepository.GetUserReservation(userId);
        }

        public async Task UpdateReserveToBorrow(int bookId,int userId)
        {
            int borrowedCount = await _reservationRepository.CountBorrowedBooks(userId);

            if (borrowedCount >= 3)
            {
                throw new InvalidOperationException("کاربر بیش از 3 کتاب را نمی تواند به امانت بگیرد.");
            }
            List < Reservation > userReservations =await _reservationRepository.GetUserReservation(userId);
            Reservation model = userReservations.FirstOrDefault(r=>r.BookId==bookId && !r.IsBorrowed && r.ReservationStatus == ReservationStatus.Active);
            model.ReserveDate = DateTime.Now;
            model.DueDate= DateTime.Now.AddDays(14);
            model.ReservationStatus = ReservationStatus.Borrowed;
            model.IsBorrowed = true;
            model.ReturnDate = null;
            await _reservationRepository.Update(model);
            Book book = await _bookRepository.GetById(bookId);
            book.BookStatus = BookStatus.Available;
            await _bookRepository.Update(book);
        }
        public async Task BorrowToReturn(int bookId, int userId)
        {
            Reservation model = await _reservationRepository.FindBorrowedBook(bookId,userId);
            model.ReturnDate= DateTime.Now;
            model.ReservationStatus=ReservationStatus.Returned;
            Book book = await _bookRepository.GetById(bookId);
            book.BookStatus=BookStatus.Available;
            await _reservationRepository.Update(model);
            await _bookRepository.Update(book);
        }

        public async Task CancelReservation(int bookId, int userId)
        {
            Reservation model = await _reservationRepository.FindReservedBook(bookId, userId);
            model.ReservationStatus = ReservationStatus.Cancelled;
            await _reservationRepository.Update(model);
        }

        public async Task<List<BookReservationViewModel>> ShowReservationsDate(DateTime date)
        {
            List<Reservation> reservations = await _reservationRepository.GetByDate(date);
            if(reservations==null)
            {
                    throw new InvalidOperationException("کاربر بیش از 3 کتاب را نمی تواند به امانت بگیرد.");
            }
            List<int> booksId = reservations.Select(x => x.BookId).Distinct().ToList();
            List<Book?> books = await _bookRepository.GetBooksReserved(booksId);

            PersianCalendar pc = new PersianCalendar();

            List<BookReservationViewModel> model = reservations
                    .Join(books, r => r.BookId, b => b.Id,
                    (r, b) => new BookReservationViewModel
                    {
                       ReserveDate= pc.GetYear(r.ReserveDate).ToString() + "/" + pc.GetMonth(r.ReserveDate).ToString() + "/" + pc.GetDayOfMonth(r.ReserveDate).ToString(),
                       Title= b.Title,
                       Author=b.Author,
                       ReturnDate = r.ReturnDate!=null? pc.GetYear(r.ReturnDate.Value).ToString() +"/"+ pc.GetMonth(r.ReturnDate.Value).ToString() + "/" + pc.GetDayOfMonth(r.ReturnDate.Value).ToString():null,
                       DueDate= pc.GetYear(r.DueDate).ToString() + "/" + pc.GetMonth(r.DueDate).ToString() + "/" + pc.GetDayOfMonth(r.DueDate).ToString(),
                       ISBN=b.ISBN,
                       Barcode=b.Barcode,
                    }).ToList();
                return model;
            
        }

        public bool AnyReserva(DateTime date)
        {
            bool IsExisted =  _reservationRepository.IsAnyReserva(date);
            return IsExisted;
        }

        public async Task<BookInfoPartial?> GetByISBN(string ISBN)
        {

            Book? book = await _bookRepository.GetByISBN(ISBN);
            if (book == null)
            {
                return null;
            }
            else
            {
                List<Reservation> bookReserves = await _reservationRepository.GetByBookId(book.Id);
                DateTime AccessDate = DateTime.MinValue;
                DateTime ReturnDate = DateTime.MinValue;
                bool IsBorrowed = false;
                bool IsReserved = false;
                bool IsOverDue = bookReserves.Any(b => b.IsBorrowed && b.DueDate < DateTime.Now && b.ReturnDate == null);
                if (IsOverDue)
                {
                    AccessDate = bookReserves.SingleOrDefault(b => b.IsBorrowed && b.DueDate < DateTime.Now && b.ReturnDate == null).ReserveDate;
                    ReturnDate = bookReserves.SingleOrDefault(b => b.IsBorrowed && b.DueDate < DateTime.Now && b.ReturnDate == null).DueDate;
                }

                IsBorrowed = bookReserves.Any(b => b.IsBorrowed && b.DueDate > DateTime.Now && b.ReturnDate == null);
                IsReserved = bookReserves.Any(b => b.ReservationStatus == ReservationStatus.Active && b.IsBorrowed == false);
                if (IsReserved)
                {
                    AccessDate = bookReserves.SingleOrDefault(b => b.ReservationStatus == ReservationStatus.Active && b.IsBorrowed == false).ReserveDate;
                    ReturnDate = bookReserves.SingleOrDefault(b => b.ReservationStatus == ReservationStatus.Active && b.IsBorrowed == false).DueDate;
                }
                else if (IsBorrowed)
                {
                    AccessDate = bookReserves.SingleOrDefault(b => b.IsBorrowed && b.DueDate > DateTime.Now && b.ReturnDate == null).ReserveDate;
                    ReturnDate = bookReserves.SingleOrDefault(b => b.IsBorrowed && b.DueDate > DateTime.Now && b.ReturnDate == null).DueDate;
                }
                BookInfoPartial bookInfo = new BookInfoPartial
                {
                    Book = book,
                    IsBorrowed = IsBorrowed,
                    HasLate = IsOverDue,
                    IsReserver = IsReserved,
                    accessDate = AccessDate,
                    returnDate = ReturnDate,
                };
                return bookInfo;

            }
        }
    }
}
