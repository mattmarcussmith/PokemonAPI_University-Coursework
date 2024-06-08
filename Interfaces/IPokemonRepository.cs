using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
        Pokemon GetPokemon(int id);
        Pokemon GetPokemon(string name);
        decimal GetPokemonRating(int rating);
        bool PokemonExists(int id);


    }
}
