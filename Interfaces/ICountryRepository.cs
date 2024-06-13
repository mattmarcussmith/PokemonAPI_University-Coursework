using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface ICountryRepository
    {
        // GET Contracts
        ICollection<Country> GetCountries();
        Country GetCountryById(int id);
        Country GetCountryByOwner(int ownerId);
        ICollection<Owner> GetOwnersByCountry(int countryId);
        bool CountryExist(int countryId);
        bool CreateCountry(Country country);
        bool UpdateCountryById(Country country);
        bool DeleteCountryById(Country country);

        bool Save();



    }
}
