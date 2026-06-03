using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Enums;
using DimainLayer.Model;

namespace DimainLayer.Repository
{
    public interface IBookRepository
    {
        //CRUD
        public Task Create(Book book);
        public Task Delete(Book book);
        public Task<List<Book>> GetAll();
        public Task<Book?> GetById(int id);
        public Task Update(Book book);

        //متدهای اختصاصی
        Task<Book?> GetByISBN(string isbn);
        Task<List<Book>> SearchByTitle(string title);
        Task<List<Book>> SearchByAuthor(string author);
        Task<List<Book>>GetByStatus(BookStatus status);
        Task<List<Book?>> GetBooksReserved(List<int> ids);
    }
}
