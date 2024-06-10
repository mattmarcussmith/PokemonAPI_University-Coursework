using AutoMapper;
using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{

    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _dataContext;



        public CountryRepository(DataContext context)
        {
            _dataContext = context;
           
            
        }
        public bool CountryExist(int countryId)
        {
            return _dataContext.Countries.Any(c => c.Id == countryId);
        }

        public ICollection<Country> GetCountries()
        {
            return _dataContext.Countries
                               .OrderBy(c => c.Id)
                               .ToList();
        }

        public Country GetCountryById(int id)
        {
            
            return _dataContext.Countries
                               .Where(c => c.Id == id)
                               .FirstOrDefault();
        }

        public Country GetCountryByOwner(int ownerId)
        {
            return _dataContext.Owners
                               .Where(o => o.Id == ownerId)
                               .Select(c => c.Country)
                               .FirstOrDefault();
        }

        public ICollection<Owner> GetOwnersByCountry(int countryId)
        {
            return _dataContext.Owners
                               .Where(c => c.Country.Id == countryId)
                               .ToList();
        }
    }
}
