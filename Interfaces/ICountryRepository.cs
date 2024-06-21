using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface ICountryRepository
    {
        Task<ICollection<Country>> GetCountries();
        Task<Country> GetCountryById(int countryId);
        Task<Country> GetCountryByOwnerId(int ownerId);
        Task<ICollection<Owner>> GetOwnersByCountryId(int countryId);
        Task<bool> CountryExist(int countryId);
        Task<bool> CreateCountry(Country country);
        Task<bool> UpdateCountry(Country country);
        Task<bool> DeleteCountry(Country country);
        Task<bool> Save();



    }
}
