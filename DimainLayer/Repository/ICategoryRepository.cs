using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;

namespace DimainLayer.Repository
{
    public interface ICategoryRepository
    {
        //CRUD
        public Task Create(Category category);
        public Task Delete(Category category);
        public Task<List<Category>> GetAll();
        public Task<Category?> GetById(int id);
        public Task Update(Category category);

        //جستوجو براساس نام دسته‌بندی
        Task<Category> GetByName(string name);
        //بررسی اینکه دسته‌بندی کتاب دارد یا نه
        Task<bool> HasBooks(int categoryId);
    }
}
