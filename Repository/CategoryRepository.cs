using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{

    public class CategoryRepository : ICategoryRepository
    {

        private readonly DataContext _dataContext;
        public CategoryRepository(DataContext dataContext)
        {
            _dataContext = dataContext;

        }

        // Get database calls
        public bool CategoryExists(int id)
        {
            return _dataContext.Categories.Any(c => c.Id == id);

        }

        public ICollection<Category> GetCategories()
        {
            return _dataContext.Categories
                               .OrderBy(c => c.Id)
                               .ToList();
        }

        public Category GetCategoryById(int id)
        {
            return _dataContext.Categories
                               .Where(c => c.Id == id)
                               .FirstOrDefault();
        }

        public ICollection<Pokemon> GetPokemonsByCategoryId(int categoryId)
        {
            return _dataContext.PokemonCategories
                               .Where(c => c.CategoryId == categoryId)
                               .Select(p => p.Pokemon)
                               .ToList();
        }

        // Create database calls
        public bool CreateCategory(Category category)
        {
        

            _dataContext.Add(category);
            return Save();
        }
        public bool UpdateCategoryById(Category category)
        {
            _dataContext.Update(category);
            return Save();
        }

        public bool DeleteCategoryById(Category category)
        {
            _dataContext.Remove(category);
            return Save();
        }

        public bool Save()
        {
            return _dataContext.SaveChanges() > 0 ? true : false;
        }

      
    }
}
