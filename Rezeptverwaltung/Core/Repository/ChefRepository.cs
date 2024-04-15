using Core.Entities;
using Core.ValueObjects;

namespace Core.Repository;

public interface ChefRepository
{
    void add(Chef chef);

    void remove(Chef chef);

    Chef? findByName(Username username);
}
