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
        public async Task<bool> CategoryExist(int categoryId)
        {
            try
            {
                return await _dataContext.Categories.AnyAsync(c => c.Id == categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to check if category with id {categoryId} exists: {ex.Message}");
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
        public async Task<Category> GetCategoryById(int categoryId)
        {
            try
            {
                return await _dataContext.Categories
                               .Where(c => c.Id == categoryId)
                               .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch category with id {categoryId}: {ex.Message}" );
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
        public async Task<bool> UpdateCategory(Category category)
        {
            try
            {
                var existingCategory = await _dataContext.Categories.Where(c => c.Id == category.Id).FirstOrDefaultAsync();
                if (existingCategory == null)
                {
                    _logger.LogError($"Category with id {category.Id} not found");
                    return false;
                }
                _dataContext.Update(existingCategory);

            } catch(Exception ex)
            {
                _logger.LogError($"Failed to update category: {ex.Message}");
                return false;
            }
            return await Save();
        }
        public async Task<bool> DeleteCategory(Category category)
        {
            try
            {
                _dataContext.Remove(category);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete category with id {category.Id}: {ex.Message}");
                return false;
            }
            return await Save();
        }
        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0 ? true : false;
        }

    }
}
