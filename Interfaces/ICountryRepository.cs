using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface ICountryRepository
    {
        Task<ICollection<Country>> GetCountries();
        Task<Country> GetCountryById(int id);
        Task<Country> GetCountryByOwner(int ownerId);
        Task<ICollection<Owner>> GetOwnersByCountryId(int countryId);
        Task<bool> CountryExist(int countryId);
        Task<bool> CreateCountry(Country country);
        Task<bool> UpdateCountryById(Country country);
        Task<bool> DeleteCountryById(Country country);
        Task<bool> Save();



    }
}
