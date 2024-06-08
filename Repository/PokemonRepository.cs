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
            return _dataContext.Pokemons.OrderBy(p => p.Id).ToList();
        }

        public Pokemon GetPokemon(int id)
        {
            return  _dataContext.Pokemons.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon GetPokemon(string name)
        {
            return _dataContext.Pokemons.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetPokemonRating(int pokemonId)
        {
            var review = _dataContext.Reviews.Where(p => p.Pokemon.Id == pokemonId);
            if(review.Count() <= 0)
            {
                return 0;
            } 
            return ((decimal)review.Sum(r => r.Rating) / review.Count());
        }
         
    
        public bool PokemonExists(int pokemonId)
        {
            return _dataContext.Pokemons.Any(p => p.Id == pokemonId);
        }
    }
}
