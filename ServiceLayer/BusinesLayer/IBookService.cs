using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;
using ServiceLayer.DTO;
using ServiceLayer.ViewModel;

namespace ServiceLayer.BusinesLayer
{
    public interface IBookService
    {
        public Task Create(BookCreateDto model);
        public Task<List<Book>> GetAll();
        public Task<List<CategoryPivotViewModel>> GetCategoryPivotReport();
        public Task<Book> GetByISBN(string isbn);
        public Task<Book?> GetById(int id);
        public Task Update(Book book);
    }
}
