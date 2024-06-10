using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface ICountryRepository
    {
        ICollection<Country> GetCountries();
        Country GetCountryById(int id);
        Country GetCountryByOwner(int ownerId);
        ICollection<Owner> GetOwnersByCountry(int countryId);
        bool CountryExist(int countryId);


    }
}
