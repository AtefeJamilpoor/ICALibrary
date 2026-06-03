using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;

namespace ServiceLayer.BusinesLayer
{
    public interface ICategoryService
    {
        public Task<List<Category>> GetAll();
        public Task<Category?> GetById(int id);
        public Task Create(Category category);
        public Task Update(Category category);
        public Task<string> GetCategoryName(int id);
    }
}
