using Microsoft.EntityFrameworkCore;
using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{
    /// <summary>
    /// Owner Repository
    /// </summary>
    public class OwnerRepository : IOwnerRepository
    { 
       
        private readonly DataContext _dataContext;
        private readonly ILogger<OwnerRepository> _logger;

        /// <summary>
        /// Owner Repository Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="logger"></param>
        public OwnerRepository(DataContext dataContext, ILogger<OwnerRepository> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        /// <summary>
        /// Owner Exist in the database
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public async Task<bool> OwnerExist(int ownerId)
        {
            try
            {
                return await _dataContext.Owners.AnyAsync(o => o.Id == ownerId);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to check if owner with id {ownerId} exists: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Get owner by id in the database
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public async Task<Owner> GetOwnerById(int ownerId)
        {
            try
            {
                return await _dataContext.Owners.Where(o => o.Id == ownerId)
                                                 .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch owner with id {ownerId}: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Get all owners from the database
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<Owner>> GetOwners()
        {
            try
            {
                return await _dataContext.Owners
                                  .OrderBy(o => o.Id)
                                  .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch owners: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// GetPokemon by owner id from the database
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public async Task<ICollection<Pokemon>> GetPokemonsByOwnerId(int ownerId)
        {
            try
            {
                return await _dataContext.PokemonOwners.Where(o => o.OwnerId == ownerId)
                                             .Select(p => p.Pokemon)
                                             .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch pokemons for owner with id {ownerId}: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Create owner in the database
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<bool> CreateOwner(Owner owner)
        {
            try
            {
                await _dataContext.AddAsync(owner);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create owner: {ex.Message}");
            }
            return await Save();
        }
        /// <summary>
        /// Update owner in the database
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<bool> UpdateOwner(Owner owner)
        {
            try
            {
                var existingOwner = await _dataContext.Owners.Where(o => o.Id == owner.Id)
                                                             .Include(o => o.Country)
                                                             .FirstOrDefaultAsync();

                if (existingOwner == null)
                {
                    _logger.LogError($"Owner with id {owner.Id} not found");
                    return false;
                }

                _dataContext.Update(existingOwner);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update owner with id {owner.Id}: {ex.Message}");
                return false;
            }
            return await Save();
        }
        /// <summary>
        /// Delete owner from the database
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<bool> DeleteOwner(Owner owner)
        {
            try
            {
                _dataContext.Remove(owner);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete owner with id {owner.Id}: {ex.Message}");

            }
            return await Save();
        }
        /// <summary>
        /// Save changes in the database
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0 ? true : false;
        }


    }
}
