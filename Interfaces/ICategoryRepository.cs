
using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetCategories();
        Task<Category> GetCategoryById(int id);
        Task<ICollection<Pokemon>> GetPokemonsByCategoryId(int categoryId);
        Task <bool> CreateCategory(Category category);
        Task<bool> UpdateCategoryById(Category category);
        Task <bool> CategoryExists(int id);
        Task<bool> DeleteCategoryById(Category category);
        Task<bool> Save();

      

    }
}
