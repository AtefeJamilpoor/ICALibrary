using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Enums;
using DimainLayer.Model;
using DimainLayer.Repository;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.DTO;
using ServiceLayer.ViewModel;

namespace ServiceLayer.BusinesLayer
{
    public class BookService:IBookService
    {
        private readonly IBookRepository _bookRepo;
        private readonly IReservationRepository _reservationRepo;
        private readonly ICategoryRepository _categoryRepo;
        public BookService(IBookRepository bookRepo, IReservationRepository reservationRepo,ICategoryRepository categoryRepository)
        {
            _bookRepo = bookRepo;
            _reservationRepo = reservationRepo;
            _categoryRepo = categoryRepository;
        }
       
        public async Task Create(BookCreateDto model)
        {
            // تولید ISBN یکتا
            var isbn = await GenerateUniqueISBNAsync();

            // تولید بارکد یکتا بر اساس ISBN
            var barcode = await GenerateUniqueBarcodeAsync(isbn);

            var category = await _categoryRepo.GetById(model.CategoryId.Value);

            var book = new Book
            {
                Title = model.Title,
                Author = model.Author,
                CategoryId = model.CategoryId.Value,
                ISBN = isbn,
                Barcode = barcode,
                BookStatus = BookStatus.Available,
                Category=category,
            };

            await _bookRepo.Create(book);
        }
        private async Task<string> GenerateUniqueISBNAsync()
        {
            bool exists;
            string isbn;

            do
            {
                string prefix = "978";
                string group = "964";
                string publisher = "123";
                string uniquePart = RandomNumberGenerator.GetInt32(100,1000).ToString();

                string partial = prefix + group + publisher + uniquePart; 

                // رقم کنترلی ساده
                int sum = partial.Where(char.IsDigit)
                                 .Sum(c => c - '0');
                int checkDigit = sum % 10;

                isbn = partial + checkDigit;

                // بررسی یکتایی در دیتابیس
                var books = await _bookRepo.GetAll();
                exists = books.Any(x => x.ISBN == isbn);
            } while (exists);

            return isbn;
        }

        private async Task<string> GenerateUniqueBarcodeAsync(string isbn)
        {
            string barcode;
            bool exists;

            do
            {
                // ترکیب ISBN + تاریخ + شناسه تصادفی
                var random = Guid.NewGuid().ToString("N").Substring(0, 6);
                barcode = $"{isbn}-{DateTime.UtcNow:yyyyMMddHHmmss}-{random}";

                // بررسی یکتایی در دیتابیس
                var books = await _bookRepo.GetAll();
                exists = books.Any(b => b.Barcode == barcode);

            } while (exists);

            return barcode;
        }
        public async Task<List<Book>> GetAll()
        {
            return await _bookRepo.GetAll();  
        }
        public async Task<List<CategoryPivotViewModel>> GetCategoryPivotReport()
        {
            var categories = await _categoryRepo.GetAll();
            var books = await GetAll();

            var totalBooks = books.Count;

            var model = books
                .Select(b => new
                {
                    CategoryName = b.Category?.Name ?? "بدون دسته‌بندی",
                    Status = b.BookStatus
                })
                .GroupBy(x => x.CategoryName)
                .Select(g => new CategoryPivotViewModel
                {
                    CategoryName = g.Key,
                    BookCount = g.Count(),
                    Percentage = totalBooks == 0 ? 0 : Math.Round((double)g.Count() / totalBooks * 100, 2),

                    AvailableCount = g.Count(x => x.Status == BookStatus.Available),
                    BorrowedCount = g.Count(x => x.Status == BookStatus.Borrowed),
                    LostCount = g.Count(x => x.Status == BookStatus.Lost)
                })
                .ToList();

            return model;
        }
        public async Task<Book?> GetByISBN(string isbn)
        {
            var book = await _bookRepo.GetByISBN(isbn);
            if (book == null)
            {
                return null;
            }
            return book;
        }
        public  async Task<Book?> GetById(int id)
        {
            Book? book = await _bookRepo.GetById(id);
            return book;
        }

        public async Task Update(Book book)
        {
            await _bookRepo.Update(book);
        }
    }
}
