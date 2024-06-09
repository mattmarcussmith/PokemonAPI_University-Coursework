
using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategoryById(int id);
        ICollection<Pokemon> GetPokemonsByCategoryId(int categoryId);
        bool CategoryExists(int id);


    }
}
