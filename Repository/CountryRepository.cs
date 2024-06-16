using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<CountryRepository> _logger;

        public CountryRepository(DataContext context, ILogger<CountryRepository> logger)
        {
            _dataContext = context;
            _logger = logger;
        }
        public async Task<bool> CountryExist(int countryId)
        {
            try
            {
                return await _dataContext.Countries.AnyAsync(c => c.Id == countryId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to check if country with id {countryId} exists: {ex.Message}");
                return false;
            }
        }
        public async Task<ICollection<Country>> GetCountries()
        {
            try
            {
                return await _dataContext.Countries
                               .OrderBy(c => c.Id)
                               .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch countries: {ex.Message}");
                return null;
            }
        }
        public async Task<Country> GetCountryById(int id)
        {
            try
            {
                return await _dataContext.Countries
                               .Where(c => c.Id == id)
                               .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch country with id {id}: {ex.Message}");
                return null;
            }
        }
        public async Task<Country> GetCountryByOwner(int ownerId)
        {
            try
            {
                return await _dataContext.Owners
                               .Where(o => o.Id == ownerId)
                               .Select(c => c.Country)
                               .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                _logger.LogError($"Owner with id: {ownerId} not found");
                return null;
            }
        }
        public async Task<ICollection<Owner>> GetOwnersByCountryId(int countryId)
        {
            try
            {
                return await _dataContext.Owners
                               .Where(c => c.Country.Id == countryId)
                               .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch owners by country id {countryId}: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> CreateCountry(Country country)
        {
            try
            {
                await _dataContext.AddAsync(country);
                return await Save();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create country: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateCountryById(Country country)
        {
            try
            {
                _dataContext.Update(country);
                return await Save();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update country with id {country.Id}: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteCountryById(Country country)
        {
            try
            {
                _dataContext.Remove(country);
                return await Save();
            }
            catch (Exception)
            {
                _logger.LogError($"Country with id: {country.Id} not found");
                return false;
            }
        }
        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
