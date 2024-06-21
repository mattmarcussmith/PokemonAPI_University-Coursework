using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface IPokemonRepository
    {
     
        Task<ICollection<Pokemon>> GetPokemons();
        Task<Pokemon> GetPokemonByName(string name);
        Task<Pokemon> GetPokemonById(int pokemonId);
        Task<decimal> GetPokemonRating(int rating);
        Task<bool> PokemonExist(int pokemonId);
        Task<bool> CreatePokemon(int ownerId, int categoryId, Pokemon pokemon);
        Task<bool> UpdatePokemon(int ownerId, int categoryId,  Pokemon pokemon);
        Task<bool> DeletePokemon(Pokemon pokemon);
        Task<bool> Save();
    }
}
