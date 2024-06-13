using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface IPokemonRepository
    {
        // Get interface contracts
        ICollection<Pokemon> GetPokemons();
        Pokemon GetPokemonById(int id);
        Pokemon GetPokemonByName(string name);
        decimal GetPokemonRating(int rating);
        bool PokemonExists(int id);

        // Create interface contracts
        bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon);
        bool UpdatePokemonById(int ownerId, int categoryId,  Pokemon pokemon);
        bool DeletePokemonById(Pokemon pokemon);


        bool Save();
    }
}
