using Core.Entities;
using Core.ValueObjects;

namespace Core.Repository;

public interface ChefRepository
{
    void save(Chef chef);

    Chef? findByName(Username username);
}
