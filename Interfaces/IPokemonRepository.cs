using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
        Pokemon GetPokemonById(int id);
        Pokemon GetPokemonByName(string name);
        decimal GetPokemonRating(int rating);
        bool PokemonExists(int id);
    }
}
