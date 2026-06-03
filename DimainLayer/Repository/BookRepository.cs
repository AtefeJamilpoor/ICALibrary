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
    public class BookRepository:IBookRepository
    {
        private readonly LibraryDBContext _dbContext;

        public BookRepository(LibraryDBContext dBContext)
        {
                _dbContext = dBContext;
        }
        
        public async Task Create(Book book)
        {
            await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Book book)
        {
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Book>> GetAll()
        {
            return await  _dbContext.Books.ToListAsync();
        }

        public async Task<Book?> GetById(int id)
        {
            return  _dbContext.Books.FirstOrDefault(b => b.Id == id);
        }

        public async Task<List<Book?>> GetBooksReserved(List<int> ids)
        {
            List<Book> books = await _dbContext.Books.Where(x=>ids.Contains(x.Id)).ToListAsync();   
            return books;
        }

        public async Task  Update(Book book)
        {
            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Book?> GetByISBN(string isbn)
        {
            return await _dbContext.Books.FirstOrDefaultAsync(x=>x.ISBN==isbn);
        }
        public async Task<List<Book>> SearchByTitle(string title)
        {
            return await _dbContext.Books.Where(x=>x.Title.Contains(title)).ToListAsync();
        }
        public async Task<List<Book>> SearchByAuthor(string author)
        {
            return await _dbContext.Books.Where(b=>b.Author.Contains(author)).ToListAsync();
        }
        public async Task<List<Book>> GetByStatus(BookStatus status)
        {
            return await _dbContext.Books.Where(b=>b.BookStatus==status).ToListAsync();
        }

    }
}
