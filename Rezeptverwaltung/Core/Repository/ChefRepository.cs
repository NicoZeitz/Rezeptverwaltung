using Core.Entities;
using Core.ValueObjects;

namespace Core.Repository;

public interface ChefRepository
{
    void Add(Chef chef);

    void Remove(Chef chef);

    Chef? FindByUsername(Username username);
}
