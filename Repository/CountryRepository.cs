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
            } catch(Exception)
            {
                _logger.LogError($"Country with id: {countryId} not found");
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
            } catch (Exception)
            {
                _logger.LogError("No countries found");
                return null;
            } 
        }
        public async Task<Country> GetCountryById(int countryId)
        {
            try
            {
                return await _dataContext.Countries
                               .Where(c => c.Id == countryId)
                               .FirstOrDefaultAsync();
            } catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch category with id {countryId}: {ex.Message}");
                return null;
            }
        }
        public async Task<Country> GetCountryByOwnerId(int ownerId)
        {
            try
            {
                return await _dataContext.Owners
                               .Where(o => o.Id == ownerId)
                               .Select(c => c.Country)
                               .FirstOrDefaultAsync();
            } catch(Exception)
            {
                _logger.LogError($"Country with owner id: {ownerId} not found");
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
            } catch(Exception)
            {
                _logger.LogError($"Country with id: {countryId} not found");
                return null;
            }
        }
        public async Task<bool> CreateCountry(Country country)
        {
            try
            {
                await _dataContext.AddAsync(country);
               
            }
            catch (Exception)
            {
                _logger.LogError("Something went wrong inside CreateCountry action");
                return false;
            }
            return await Save();
        }
        public async Task<bool> UpdateCountry(Country country)
        {
            try
            {
                var existingCountry = await _dataContext.Countries.Where(c => c.Id == country.Id).FirstOrDefaultAsync();
                if (existingCountry == null)
                {
                    _logger.LogError($"Country with id: {country.Id} not found");
                    return false;
                }

                _dataContext.Update(existingCountry);
            } catch(Exception ex)
            { 
                _logger.LogError($"Failed to update country: {ex.Message}");
                return false;
            }
            return await Save();

        }
        public async Task<bool> DeleteCountry(Country country)
        {
            try
            {
                var countries = await _dataContext.Countries
                                                        .Where(c => c.Id == country.Id).FirstOrDefaultAsync();
                var owners = await _dataContext.Owners
                                              .Where(o => o.Country.Id == country.Id)
                                              .ToListAsync();

                if (countries != null)
                {

                    _dataContext.RemoveRange(owners);
                    _dataContext.Remove(countries);
                   
                }
                else
                {
                    _logger.LogError($"Country with id: {country.Id} not found");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete country{country.Id} : {ex.Message}");
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
