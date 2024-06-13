using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface IOwnerRepository
    {
        ICollection<Owner> GetOwners();
        Owner GetOwnerById(int ownerId);
        ICollection<Pokemon> GetPokemonsByOwner(int ownerId);

        bool OwnerExists(int ownerId); 
        bool CreateOwner(Owner owner);

        bool UpdateOwnerById(Owner owner);
        bool DeleteOwnerById(Owner owner);


        bool Save();

    }
}
