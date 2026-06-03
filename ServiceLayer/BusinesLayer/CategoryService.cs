using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;
using DimainLayer.Repository;

namespace ServiceLayer.BusinesLayer
{
    public class CategoryService:ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {    
            _categoryRepository = categoryRepository;
        }
        public async Task<List<Category>> GetAll()
        {
            return await _categoryRepository.GetAll();
        }
        public async Task<Category?> GetById(int id)
        {
            return await _categoryRepository.GetById(id);
        }
        public async Task<string> GetCategoryName(int id)
        {
            var category = await GetById(id);
            return category.Name;
        }
        public async Task Create(Category category)
        {
             await _categoryRepository.Create(category);
        }
        public async Task Update(Category category)
        {
           await  _categoryRepository.Update(category);
        }
    }
}
