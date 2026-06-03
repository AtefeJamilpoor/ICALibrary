using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Contex;
using DimainLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace DimainLayer.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly LibraryDBContext _db;
        public CategoryRepository(LibraryDBContext db)
        {
                _db = db;
        }
        public async Task Create(Category category)
        {
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(Category category)
        {
            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAll()
        {
            return await _db.Categories.ToListAsync();
        }

        public async Task<Category?> GetById(int id)
        {
            return await _db.Categories.FirstOrDefaultAsync(x=> x.Id == id);
        }
        public async Task Update(Category category)
        {
            _db.Categories.Update(category);
            await _db.SaveChangesAsync();
        }
        public async Task<Category?> GetByName(string name)
        {
            return await _db.Categories.FirstOrDefaultAsync(x=>x.Name==name);
        }

        public async Task<bool> HasBooks(int categoryId)
        {
            return await _db.Books.AnyAsync(x=>x.CategoryId==categoryId);
        }

        
    }
}
