using Microsoft.EntityFrameworkCore;
using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{
    public class CategoryRepository : ICategoryRepository
    {

        private readonly DataContext _dataContext;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(DataContext dataContext, ILogger<CategoryRepository> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }
        public async Task<bool> CategoryExists(int id)
        {
            try
            {
                return await _dataContext.Categories.AnyAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to check if category with id {id} exists: {ex.Message}");
                return false;
            }
        }
        public async Task<ICollection<Category>> GetCategories()
        {
            try
            {
                return await _dataContext.Categories
                                         .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch categories: {ex.Message}");
                return null;
            }
        }
        public async Task<Category> GetCategoryById(int id)
        {
            try
            {
                return await _dataContext.Categories
                               .Where(c => c.Id == id)
                               .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch category with id {id}: {ex.Message}" );
                return null;
            }
        }

        public async Task<ICollection<Pokemon>> GetPokemonsByCategoryId(int categoryId)
        {
            try
            {
                return await _dataContext.PokemonCategories
                             .Where(c => c.CategoryId == categoryId)
                             .Select(p => p.Pokemon)
                             .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch pokemon by category id {categoryId}: {ex.Message }");
                return null;
            }
          
        }

        public async Task<bool> CreateCategory(Category category)
        {
            try
            {
                await _dataContext.AddAsync(category);
                return await Save();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create category: {ex.Message}");
                return false;
            }

        }
        public async Task<bool> UpdateCategoryById(Category category)
        {
            try
            {
                _dataContext.Update(category);
                return await Save();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update category: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteCategoryById(Category category)
        {
            try
            {
                _dataContext.Remove(category);
                return await Save();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete category: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0 ? true : false;
        }

    }
}
