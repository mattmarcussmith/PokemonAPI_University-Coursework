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

        // Get database calls
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

        // Create database calls
        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            // Updating the PokemonOwner and PokemonCategory tables
            var pokemonOwnerEntity = _dataContext.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
            var pokemonCategoryEntity = _dataContext.Categories.Where(c => c.Id == categoryId).FirstOrDefault();


            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon,
            };
            _dataContext.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory()
            {
                Category = pokemonCategoryEntity,
                Pokemon = pokemon,
            };
            _dataContext.Add(pokemonCategory);
            _dataContext.Add(pokemon);

            return Save();


        }

        public bool UpdatePokemonById(int ownerId, int categoryId, Pokemon pokemon)
        {
            _dataContext.Update(pokemon);
            return Save();
        }

        public bool DeletePokemonById(Pokemon pokemon)
        {
            _dataContext.Remove(pokemon);
            return Save();
        }
        public bool Save()
        {
            return _dataContext.SaveChanges() > 0 ? true : false;
        }

      
    }
}
