
using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategoryById(int id);
        ICollection<Pokemon> GetPokemonsByCategoryId(int categoryId);
        bool CreateCategory(Category category);
        bool UpdateCategoryById(Category category);
        bool CategoryExists(int id);
        bool DeleteCategoryById(Category category);
        bool Save();

    }
}
