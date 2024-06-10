using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _dataContext;
        public PokemonRepository(DataContext dataContext)
        {
          _dataContext = dataContext;

        }
        public ICollection<Pokemon> GetPokemons()
        {
            return _dataContext.Pokemons
                               .OrderBy(p => p.Id)
                               .ToList();
        }

        public Pokemon GetPokemonById(int id)
        {
            return  _dataContext.Pokemons
                                .Where(p => p.Id == id)
                                .FirstOrDefault();
        }

        public Pokemon GetPokemonByName(string name)
        {
            return _dataContext.Pokemons
                               .Where(p => p.Name == name)
                               .FirstOrDefault();
        }

        public decimal GetPokemonRating(int pokemonId)
        {
            var pokemonRating = _dataContext.Reviews
                                            .Where(r => r.Pokemon.Id == pokemonId)
                                            .Select(r => (decimal?)r.Rating)
                                            .Average() ?? 0;
           
            return pokemonRating;
        }
         
        public bool PokemonExists(int pokemonId)
        {
            return _dataContext.Pokemons.Any(p => p.Id == pokemonId);
        }
    }
}
