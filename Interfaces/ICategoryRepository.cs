
using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetCategories();
        Task<Category> GetCategoryById(int categoryId);
        Task<ICollection<Pokemon>> GetPokemonsByCategoryId(int categoryId);
        Task <bool> CreateCategory(Category category);
        Task<bool> UpdateCategory(Category category);
        Task <bool> CategoryExist(int categoryId);
        Task<bool> DeleteCategory(Category category);
        Task<bool> Save();

      

    }
}
