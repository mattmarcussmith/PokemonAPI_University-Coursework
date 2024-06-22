using Microsoft.EntityFrameworkCore;
using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<PokemonRepository> _logger;
        public PokemonRepository(DataContext dataContext, ILogger<PokemonRepository> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }
        public async Task<ICollection<Pokemon>> GetPokemons()
        {
            try
            {
                return await _dataContext.Pokemons
                               .OrderBy(p => p.Id)
                               .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch pokemons: {ex.Message}");
                return null;
            }
        }
        public async Task<Pokemon> GetPokemonById(int pokemonId)
        {
            try
            {
                return await _dataContext.Pokemons
                               .Where(p => p.Id == pokemonId)
                               .FirstOrDefaultAsync();
            } catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch pokemon with id {pokemonId}: {ex.Message}");
                return null;
            }
        }
        public async Task<Pokemon> GetPokemonByName(string name)
        {
            try
            {
                return await _dataContext.Pokemons
                               .Where(p => p.Name == name.ToLower().Trim())
                               .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch pokemon with name {name}: {ex.Message}");
                return null;
            }
        }
        public async Task<decimal> GetPokemonRating(int pokemonId)
        {
            try
            {
                var pokemonRating = await _dataContext.Reviews
                                            .Where(r => r.Pokemon.Id == pokemonId)
                                            .Select(r => (decimal?) r.Rating)
                                            .AverageAsync() ?? 0;
                return pokemonRating;
            } catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch rating for pokemon with id {pokemonId}: {ex.Message}");
                return 0;
            }
        }
        public async Task<bool> PokemonExist(int pokemonId)
        {
            try
            {
                return await _dataContext.Pokemons.AnyAsync(p => p.Id == pokemonId);

            } catch (Exception ex)
            {
                _logger.LogError($"Failed to check if pokemon with id {pokemonId} exists: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            try
            {
            
                var pokemonOwnerEntity = await _dataContext.Owners
                                          .Where(o => o.Id == ownerId)
                                          .FirstOrDefaultAsync();
                var pokemonCategoryEntity = await _dataContext.Categories
                                            .Where(c => c.Id == categoryId)
                                            .FirstOrDefaultAsync();
                if (pokemonOwnerEntity == null || pokemonCategoryEntity == null)
                {
                    _logger.LogError($"Owner with id {ownerId} or category with id {categoryId} does not exist");
                }
                var pokemonOwner = new PokemonOwner()
                {
                    OwnerId = ownerId,
                    Pokemon = pokemon,
                };

                var pokemonCategory = new PokemonCategory()
                {
                    CategoryId = categoryId,
                    Pokemon = pokemon,
                };

                await _dataContext.AddAsync(pokemon);
                await _dataContext.AddAsync(pokemonOwner);
                await _dataContext.AddAsync(pokemonCategory);
             
                return await Save();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add pokemon: {ex.Message}");
                return false;
            }
        } 
        public async Task<bool> UpdatePokemon( int ownerId, int categoryId, Pokemon pokemon)
        {
            try
            {

                _dataContext.Update(pokemon);
   
            } catch(Exception ex)
            {
                _logger.LogError($"Failed to update pokemon: {ex.Message}");
                return false;
            }

            return await Save();
        }
        public async Task<bool> DeletePokemon(Pokemon pokemon)
        {
            try
            {
                _dataContext.Remove(pokemon);
            } catch (Exception ex)
            {
                _logger.LogError($"Failed to delete pokemon {pokemon.Id}: {ex.Message}");
                return false;
            }
            return await Save();
        }
        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
